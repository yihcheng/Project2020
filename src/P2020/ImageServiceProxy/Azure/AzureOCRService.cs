using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using P2020.Abstraction;

namespace P2020.ImageServiceProxy.Azure
{
    internal class AzureOCRService : ICloudOCRService
    {
        private readonly string _azureOCRUrl;
        private const string _requestHeaderKeyword = "Ocp-Apim-Subscription-Key";
        private readonly string _azureOCRKey;
        private const string _providerName = "Azure";
        private readonly IComputer _computer;
        private readonly IOCRResultTextFinder _textFinder;
        private readonly ILogger _logger;
        private readonly IEngineConfig _config;
        private const string _artifactsFolderConfigKey = "ArtifactsFolder";

        public AzureOCRService(IComputer computer,
                               IOCRResultTextFinder textFinder,
                               string serviceUrl,
                               string serviceKey,
                               ILogger logger,
                               IEngineConfig config)
        {
            _computer = computer;
            _textFinder = textFinder;
            _azureOCRUrl = serviceUrl;
            _azureOCRKey = serviceKey;
            _logger = logger;
            _config = config;
        }

        public string ProviderName => _providerName;

        private async Task<string> GetOCRJsonResultAsync(string imageFile)
        {
            HttpClient httpClient = null;

            try
            {
                byte[] imageBytes = File.ReadAllBytes(imageFile);
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

                int count = 0;
                string jsonString;

                while (true)
                {
                    HttpResponseMessage responseMessage = await httpClient.GetAsync(textOperationLocation).ConfigureAwait(false);
                    jsonString = await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
                    JObject jObj = JObject.Parse(jsonString);
                    string status = (string)jObj["status"];

                    if (string.Equals(status, "Succeeded", StringComparison.OrdinalIgnoreCase))
                    {
                        return jsonString;
                    }

                    if (count == 30)
                    {
                        break;
                    }

                    await Task.Delay(100).ConfigureAwait(false);

                    count++;
                }
            }
            catch (HttpRequestException httpRequestEx)
            {
                _logger.WriteError($"{ServiceResource.NetworkErrorOnHttpRequest}  {httpRequestEx}");
            }
            catch (Exception ex)
            {
                _logger.WriteError($"{ServiceResource.AzureOCRGeneralError}  {ex}");
            }
            finally
            {
                httpClient?.Dispose();
            }

            return "";
        }

        public async Task<IScreenArea> GetOCRResultAsync(string imageFile, string textToSearch, IReadOnlyList<ScreenSearchArea> searchAreas)
        {
            if (string.IsNullOrEmpty(imageFile) || string.IsNullOrEmpty(textToSearch))
            {
                _logger.WriteError(ServiceResource.EmptyImageFileSearchText);
                return null;
            }

            if (!File.Exists(imageFile))
            {
                _logger.WriteError(ServiceResource.ImageFileNotExist);
                return null;
            }

            string jsonResult = await GetOCRJsonResultAsync(imageFile).ConfigureAwait(false);

            if (string.IsNullOrEmpty(jsonResult))
            {
                _logger.WriteError(ServiceResource.AzureEmtpyOCRResult);
                return null;
            }

            foreach (ScreenSearchArea searchArea in searchAreas)
            {
                if (_textFinder.TrySearchText(textToSearch, jsonResult, _computer.Screen, searchArea, out IScreenArea area))
                {
                    return area;
                }
            }

            // write down info for debugging records
            try
            {
                string ocrFailFileName = $"{DateTime.Now.ToString("yyyyMMddHHmmss")}_{_providerName}_TextNotFound_[{textToSearch}].txt";
                string artifactsFolder = Path.Combine(".\\", _config[_artifactsFolderConfigKey]);

                // When engine has been started, artifact folder is ensured. It should be there. 
                string ocrFailFilePath = Path.Combine(artifactsFolder, ocrFailFileName);
                File.WriteAllText(ocrFailFilePath, jsonResult);

                // put message on image
                string message = string.Format(ServiceResource.TextNotFoundInJsonResult, textToSearch, searchAreas, ocrFailFileName);
                _logger.WriteError(message);
            }
            finally
            { }

            return null;
        }
    }
}
