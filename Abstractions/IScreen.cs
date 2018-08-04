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

    public interface IScreenLocation
    {
        int X { get; }
        int Y { get; }
    }
}
