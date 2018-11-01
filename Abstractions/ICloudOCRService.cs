using System.Collections.Generic;
using System.Threading.Tasks;

namespace Abstractions
{
    public interface ICloudOCRService
    {
        string ProviderName { get; }
        Task<IScreenArea> GetOCRResultAsync(string imageFile, string textToSearch, IReadOnlyList<ScreenSearchArea> searchAreas);
    }

    public interface IOpenCVSUtils
    {
        (double Confidence, int X, int Y, int Width, int Height)? TemplateMatch(byte[] search, byte[] template);
        void DrawRedRectangle(string ImageFile, int X, int Y, int Width, int Height);
        void PutText(string ImageFile, int X, int Y, string Message);
    }
}
