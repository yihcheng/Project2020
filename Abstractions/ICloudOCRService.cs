using System.Threading.Tasks;

namespace Abstractions
{
    public interface ICloudOCRService
    {
        string ProviderName { get; }
        Task<IScreenArea> GetOCRResultAsync(string imageFile, string textToSearch, ScreenSearchArea searchArea);
        (double, int, int)? TemplateMatch(byte[] search, byte[] template);
    }

    public interface IOpenCVService
    {
        (double, int, int)? TemplateMatch(byte[] search, byte[] template);
        void DrawRedRectangle(string imageFile, int X, int Y, int width, int height);
        void PutText(string imageFile, int X, int Y, string message);
    }
}
