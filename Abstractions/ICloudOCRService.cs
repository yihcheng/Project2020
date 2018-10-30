using System.Collections.Generic;
using System.Threading.Tasks;

namespace Abstractions
{
    public interface ICloudOCRService
    {
        string ProviderName { get; }
        IOpenCVSUtils OpenCVUtils { get; }
        Task<IScreenArea> GetOCRResultAsync(string imageFile, string textToSearch, IReadOnlyList<ScreenSearchArea> searchAreas);
    }

    public interface IOpenCVSUtils
    {
        (double, int, int)? TemplateMatch(byte[] search, byte[] template);
        void DrawRedRectangle(string imageFile, int X, int Y, int width, int height);
        void PutText(string imageFile, int X, int Y, string message);
    }
}
