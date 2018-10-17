using Abstractions;

namespace ImageServiceProxy
{
    public class ScreenArea : IScreenArea
    {
        public ScreenArea(int top, int left, int bottom, int right)
        {
            Top = top;
            Left = left;
            Bottom = bottom;
            Right = right;
        }

        public int Top { get; }

        public int Left { get; }

        public int Bottom { get; }

        public int Right { get; }

        public int Width => Right - Left;

        public int Height => Bottom - Top;

        public (int X, int Y) GetCentralPoint()
        {
            int X = (Left + Right) / 2;
            int Y = (Top + Bottom) / 2;

            return (X, Y);
        }
    }
}
