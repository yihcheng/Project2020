using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Abstractions;
using ImageServiceProxy.Utils;
using Newtonsoft.Json.Linq;

namespace ImageServiceProxy.Azure
{
    internal class AzureOCRService : IImageService
    {
        // Let me know if you need the source code of those services.
        private readonly string _azureOCRUrl;
        private const string _requestHeaderKeyword = "Ocp-Apim-Subscription-Key";
        private readonly string _azureOCRKey;
        private const string _providerName = "Azure";
        private readonly IComputer _computer;
        private readonly IOCRResultTextFinder _textFinder;
        private readonly IOpenCVService _opencvService;
        private readonly ILogger _logger;

        public AzureOCRService(IComputer computer,
                               IOCRResultTextFinder textFinder,
                               IOpenCVService opencvService,
                               string serviceUrl,
                               string serviceKey,
                               ILogger logger)
        {
            _computer = computer;
            _textFinder = textFinder;
            _opencvService = opencvService;
            _azureOCRUrl = serviceUrl;
            _azureOCRKey = serviceKey;
            _logger = logger;
        }

        public string ProviderName => _providerName;

        private async Task<string> GetOCRJsonResultAsync(string imageFile)
        {
            HttpClient httpClient = null;

            try
            {
                byte[] imageBytes = File.ReadAllBytes(imageFile);
                string jsonString;
                httpClient = new HttpClient();
                string textOperationLocation = "";

                using (ByteArrayContent content = new ByteArrayContent(imageBytes))
                {
                    httpClient.DefaultRequestHeaders.Add(_requestHeaderKeyword, _azureOCRKey);
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                    HttpResponseMessage response = await httpClient.PostAsync(_azureOCRUrl, content).ConfigureAwait(false);
                    await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                    if (response.StatusCode != HttpStatusCode.Accepted)
                    {
                        return "";
                    }

                    textOperationLocation = response.Headers.FirstOrDefault(x => x.Key == "Operation-Location").Value?.FirstOrDefault();
                }

                if (string.IsNullOrEmpty(textOperationLocation))
                {
                    return "";
                }

                string status = "";
                int count = 0;

                while (true)
                {
                    HttpResponseMessage responseMessage = await httpClient.GetAsync(textOperationLocation).ConfigureAwait(false);
                    jsonString = await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
                    JObject jObj = JObject.Parse(jsonString);
                    status = (string)jObj["status"];

                    if (string.Equals(status, "Succeeded", StringComparison.OrdinalIgnoreCase))
                    {
                        return jsonString;
                    }

                    if (string.Equals(status, "Failed", StringComparison.OrdinalIgnoreCase))
                    {
                        return "";
                    }

                    if (count == 30)
                    {
                        break;
                    }

                    count++;
                }
            }
            catch (Exception ex)
            {
                _logger.WriteError("AzureOCRService GetOCRJsonResultAsync() error = " + ex);
            }
            finally
            {
                httpClient?.Dispose();
            }

            return "";
        }

        public async Task<IScreenLocation> GetOCRResultAsync(string imageFile, string textToSearch, ScreenSearchArea searchArea)
        {
            if (string.IsNullOrEmpty(imageFile) || string.IsNullOrEmpty(textToSearch))
            {
                _logger.WriteError("Image file or search text is empty!");
                return null;
            }

            if (!File.Exists(imageFile))
            {
                _logger.WriteError("Image file does not exist in file system!");
                return null;
            }

            string jsonResult = await GetOCRJsonResultAsync(imageFile).ConfigureAwait(false);

            if (string.IsNullOrEmpty(jsonResult))
            {
                _logger.WriteError("Json result is empty from OCR service. OCR got failure!");
                return null;
            }

            bool isLine = textToSearch.Trim().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Length > 1;

            if (_textFinder.TrySearchText(textToSearch, jsonResult, _computer.Screen, searchArea, out IScreenLocation location))
            {
                return location;
            }

            try
            {
                string ocrFailFileName = $"OCRfail{DateTime.Now.ToString("yyyyMMddHHmmss")}-{textToSearch}.txt";
                string ocrFailFilePath = $".\\OCRfail\\{_providerName}-{ocrFailFileName}";
                FileUtility.CreateParentFolder(ocrFailFilePath);
                File.WriteAllText(ocrFailFilePath, jsonResult);
                _logger.WriteError($"Cannot find text ({textToSearch}) in json result at {searchArea}. json is saved in {ocrFailFileName}");
            }
            catch
            { }

            return null;
        }

        public (double, int, int)? TemplateMatch(byte[] search, byte[] template)
        {
            return _opencvService.TemplateMatch(search, template);
        }
    }
}
