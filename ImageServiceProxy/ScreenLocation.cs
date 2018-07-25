using CommonContracts;

namespace ImageServiceProxy
{
    public class ScreenLocation : IScreenLocation
    {
        public ScreenLocation(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; }
        public int Y { get; }
    }
}
