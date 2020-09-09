namespace P2020.ImageServiceProxy
{
    public interface IOCRResultTextFinder
    {
        bool TrySearchText(string textToSearch, string jsonResult, IScreen screen, ScreenSearchArea searchArea, out IScreenArea area);
    }
}
