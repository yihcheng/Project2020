using System;
using P2020.Abstraction;

namespace P2020.WindowsComputer
{
    public class WindowsScreen : IScreen
    {
        private readonly ImageFormat _imgFormat;
        private static IntPtr fullscreen;
        private static WindowsAPIHelper.Rect windowRect;

        static WindowsScreen()
        {
            fullscreen = WindowsAPIHelper.GetDesktopWindow();
            windowRect = new WindowsAPIHelper.Rect();
            WindowsAPIHelper.GetWindowRect(fullscreen, ref windowRect);
        }

        public WindowsScreen()
        {
            Width = windowRect.right - windowRect.left;
            Height = windowRect.bottom - windowRect.top;

            // default to JPG
            _imgFormat = ImageFormat.Jpeg;
        }

        public int Width { get; }

        public int Height { get; }

        public bool IsSearchAreaMatch(ScreenSearchArea screenSearchArea, (int X, int Y) location)
        {
            if (screenSearchArea == ScreenSearchArea.FullScreen) return true;

            bool isTop = location.Y <= Height / 2;
            bool isLeft = location.X <= Width / 2;

            switch (screenSearchArea)
            {
                case ScreenSearchArea.Top:
                    return isTop;
                case ScreenSearchArea.Down:
                    return !isTop;
                case ScreenSearchArea.Left:
                    return isLeft;
                case ScreenSearchArea.Right:
                    return !isLeft;
                case ScreenSearchArea.TopLeft:
                    return isTop && isLeft;
                case ScreenSearchArea.TopRight:
                    return isTop && !isLeft;
                case ScreenSearchArea.DownLeft:
                    return !isTop && isLeft;
                case ScreenSearchArea.DownRight:
                    return !isTop && !isLeft;
                case ScreenSearchArea.Center:
                    return IsInCenter(location.X, location.Y);
            }

            return false;
        }

        private bool IsInCenter(int X, int Y)
        {
            bool isXCetner = X >= Width / 4 && X <= Width * 3 / 4;
            bool isYCenter = Y >= Height / 4 && Y <= Height * 3 / 4;

            return isXCetner && isYCenter;
        }

        public void SaveFullScreenAsFile(string filePathToSave)
        {
            if (string.IsNullOrEmpty(filePathToSave))
            {
                // TODO : warning ?
                return;
            }

            try
            {
                Image fullScreenShot = CaptureFullScreenShot();
                fullScreenShot.Save(filePathToSave, _imgFormat);
            }
            finally
            { }
        }

        private Image CaptureFullScreenShot()
        {
            IntPtr fullscreen = WindowsAPIHelper.GetDesktopWindow();

            // get te hDC of the target window
            IntPtr hdcSrc = WindowsAPIHelper.GetWindowDC(fullscreen);

            // create a device context we can copy to
            IntPtr hdcDest = WindowsAPIHelper.CreateCompatibleDC(hdcSrc);

            // create a bitmap we can copy it to,
            IntPtr hBitmap = WindowsAPIHelper.CreateCompatibleBitmap(hdcSrc, Width, Height);

            // select the bitmap object
            IntPtr hOld = WindowsAPIHelper.SelectObject(hdcDest, hBitmap);

            // bitblt over
            WindowsAPIHelper.BitBlt(hdcDest, 0, 0, Width, Height, hdcSrc, 0, 0, WindowsAPIHelper.SRCCOPY);

            // restore selection
            WindowsAPIHelper.SelectObject(hdcDest, hOld);

            // clean up 
            WindowsAPIHelper.DeleteDC(hdcDest);
            WindowsAPIHelper.ReleaseDC(fullscreen, hdcSrc);

            // get a .NET image object for it
            Image img = Image.FromHbitmap(hBitmap);
            // free up the Bitmap object
            WindowsAPIHelper.DeleteObject(hBitmap);

            return img;
        }
    }
}
