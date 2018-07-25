using System.Threading.Tasks;

namespace CommonContracts
{
    public interface IImageService
    {
        Task<ITemplateMatchResult> TemplateMatch(string searchFile, string templateFile);
        Task<string> AzureOCR(string imageFile);
        Task<string> AzureRecognizeTextAsync(string imageFile);
    }
}
