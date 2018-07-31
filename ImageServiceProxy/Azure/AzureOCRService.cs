using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using CommonContracts;
using ImageServiceProxy.Utils;
using Newtonsoft.Json.Linq;

namespace ImageServiceProxy.Azure
{
    internal class AzureOCRService : IImageService
    {
        // Let me know if you need the source code of those services.
        private readonly string _azureOCRUrl; // = "https://eastasia.api.cognitive.microsoft.com/vision/v2.0/recognizeText?mode=Printed";
        private const string _requestHeaderKeyword = "Ocp-Apim-Subscription-Key";
        private readonly string _azureOCRKey; //= "1ad450620bb545e69f9ea2294a795e47";
        private const string _providerName = "Azure";
        private readonly IComputer _computer;
        private readonly IOCRResultTextFinder _textFinder;
        private readonly IOpenCVService _opencvService;

        public AzureOCRService(IComputer computer, IOCRResultTextFinder textFinder, IOpenCVService opencvService, string serviceUrl, string serviceKey)
        {
            _computer = computer;
            _textFinder = textFinder;
            _opencvService = opencvService;
            _azureOCRUrl = serviceUrl;
            _azureOCRKey = serviceKey;
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
                await Task.Delay(2000).ConfigureAwait(false);
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

                    await Task.Delay(1000).ConfigureAwait(false);

                    if (count == 30)
                    {
                        break;
                    }

                    count++;
                }
            }
            catch (Exception ex)
            {
                // TODO: log?

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
                // TODO : log?
                return null;
            }

            if (!File.Exists(imageFile))
            {
                // TODO : log?
                return null;
            }

            string jsonResult = await GetOCRJsonResultAsync(imageFile).ConfigureAwait(false);
            bool isLine = textToSearch.Trim().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Length > 1;

            if (_textFinder.TrySearchText(textToSearch, jsonResult, _computer.Screen, searchArea, out IScreenLocation location))
            {
                return location;
            }

            try
            {
                // TODO : log fail to find.
                string ocrFailFileName = $"OCRfail{DateTime.Now.ToString("yyyyMMddHHmmss")}-{textToSearch}.txt";
                // TODO : folder from config ?
                string ocrFailFilePath = $".\\OCRfail\\{_providerName}-{ocrFailFileName}";
                FileUtility.CreateParentFolder(ocrFailFilePath);
                File.WriteAllText(ocrFailFilePath, jsonResult);
            }
            catch
            { }

            return null;
        }

        public (double, int, int)? TemplateMatch(byte[] search, byte[] template)
        {
            // TODO : log ?

            return _opencvService.TemplateMatch(search, template);
        }
    }
}
