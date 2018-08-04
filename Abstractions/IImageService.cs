using System.Threading.Tasks;

namespace Abstractions
{
    public interface IImageService
    {
        string ProviderName { get; }
        Task<IScreenLocation> GetOCRResultAsync(string imageFile, string textToSearch, ScreenSearchArea searchArea);
        (double, int, int)? TemplateMatch(byte[] search, byte[] template);
    }

    public interface IOpenCVService
    {
        (double, int, int)? TemplateMatch(byte[] search, byte[] template);
    }
}
