using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using CommonContracts;
using Newtonsoft.Json;

namespace ImageServiceProxy
{
    public class ImageService : IImageService
    {
        // Let me know if you need the source code of those services.
        private static readonly string _templateMatchUrl = "http://bruceopencv.azurewebsites.net/api/opencv/templatematch";
        private static readonly string _azureOCRUrl = "http://bruceopencv.azurewebsites.net/api/azureocr/v1";
        private static readonly string _azureRecognizeTextUrl = "http://bruceopencv.azurewebsites.net/api/recognizetext/v2";

        public async Task<ITemplateMatchResult> TemplateMatch(string searchFile, string templateFile)
        {
            // for small image
            byte[] templateBytes = File.ReadAllBytes(templateFile);
            byte[] searchBytes = File.ReadAllBytes(searchFile);

            HttpClient client = new HttpClient();
            MultipartFormDataContent multipartContent = new MultipartFormDataContent();
            ByteArrayContent searchFileContent = new ByteArrayContent(searchBytes);
            multipartContent.Add(searchFileContent, "searchfile", "search1.jpg");
            ByteArrayContent templateFileContent = new ByteArrayContent(templateBytes);
            multipartContent.Add(templateFileContent, "templatefile", "template1.jpg");
            HttpResponseMessage response = await client.PostAsync(_templateMatchUrl, multipartContent).ConfigureAwait(false);
            string result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            TemplateMatchResult templateResult = JsonConvert.DeserializeObject<TemplateMatchResult>(result);

            return templateResult;
        }

        public Task<string> AzureOCR(string imageFile) => SendImageToAzure(imageFile, _azureOCRUrl);

        public Task<string> AzureRecognizeTextAsync(string imageFile) => SendImageToAzure(imageFile, _azureRecognizeTextUrl);

        private static async Task<string> SendImageToAzure(string imageFile, string azureServiceUrl)
        {
            byte[] imageFileBytes = File.ReadAllBytes(imageFile);
            HttpClient client = new HttpClient();
            MultipartFormDataContent multipartContent = new MultipartFormDataContent();
            ByteArrayContent imageFileContent = new ByteArrayContent(imageFileBytes);
            multipartContent.Add(imageFileContent, "imagefile", "template.jpg");
            HttpResponseMessage response = await client.PostAsync(azureServiceUrl, multipartContent).ConfigureAwait(false);
            return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        }
    }
}
