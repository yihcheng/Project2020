namespace Abstractions
{
    public interface IMouse
    {
        void MoveTo(int x, int y);
        void Click(int x, int y);
        void RightClick(int x, int y);
        void DoubleClick(int x, int y);
    }
}
