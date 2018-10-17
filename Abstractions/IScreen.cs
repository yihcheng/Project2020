namespace Abstractions
{
    public interface IScreen
    {
        int Width { get; }
        int Height { get; }
        byte[] CaptureScreenShot();
        void SaveFullScreenAsFile(string filePathToSave);
        bool IsSearchAreaMatch(ScreenSearchArea screenSearchArea, (int X, int Y) location);
    }

    public enum ScreenSearchArea
    {
        Right = 0,
        Left,
        Top,
        Down,
        TopRight,
        TopLeft,
        DownRight,
        DownLeft,
        Center,
        FullScreen
    }

    public interface IScreenArea
    {
        (int X, int Y) GetCentralPoint();
        int Top { get; }
        int Left { get; }
        int Bottom { get; }
        int Right { get; }
        int Width { get; }
        int Height { get; }
    }
}
