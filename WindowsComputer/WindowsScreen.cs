using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using Abstractions;

namespace WindowsComputer
{
    public class WindowsScreen : IScreen
    {
        private byte[] _imgBytes;
        private readonly ImageFormat _imgFormat;

        public WindowsScreen()
        {
            // capture full screen, instead of WorkingArea
            Width = Screen.PrimaryScreen.Bounds.Width;
            Height = Screen.PrimaryScreen.Bounds.Height;
            _imgFormat = ImageFormat.Jpeg;
        }

        public int Width { get; }

        public int Height { get; }

        public byte[] CaptureScreenShot()
        {
            Bitmap bitmapImage;

            try
            {
                bitmapImage = new Bitmap(Width, Height);
                Size size = new Size(bitmapImage.Width, bitmapImage.Height);
                Graphics graphics = Graphics.FromImage(bitmapImage);
                graphics.CopyFromScreen(0, 0, 0, 0, size);
                using (MemoryStream mstream = new MemoryStream())
                {
                    bitmapImage.Save(mstream, _imgFormat);
                    _imgBytes = mstream.ToArray();
                }
            }
            catch
            {
                // TODO: log ?
            }
            
            return _imgBytes;
        }

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
                throw new ArgumentException();
            }

            byte[] screenShotBytes = CaptureScreenShot();

            if (screenShotBytes?.Length > 0)
            {
                using (MemoryStream mstream = new MemoryStream(_imgBytes))
                {
                    Bitmap bitmap = new Bitmap(mstream);
                    bitmap.Save(filePathToSave, _imgFormat);
                }
            }
        }
    }
}
