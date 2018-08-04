using Abstractions;

namespace ImageServiceProxy
{
    public interface IOCRResultTextFinder
    {
        bool TrySearchText(string textToSearch, string jsonResult, IScreen screen, ScreenSearchArea searchArea, out IScreenLocation location);
    }
}
