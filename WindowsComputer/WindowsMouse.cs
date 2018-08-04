using System.Runtime.InteropServices;
using System.Threading;
using Abstractions;

namespace WindowsComputer
{
    public class WindowsMouse : IMouse
    {
        [DllImport("user32.dll")]
        private static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        [DllImport("user32.dll")]
        private static extern bool SetCursorPos(int x, int y);

        //Mouse actions
        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const int MOUSEEVENTF_RIGHTUP = 0x10;

        public void DoubleClick(int x, int y)
        {
            Click(x, y);
            // it is important to wait a small time slot here
            Thread.Sleep(100);
            Click(x, y);
        }

        public void Click(int x, int y)
        {
            mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, x, y, 0, 0);
        }

        public void MoveTo(int x, int y)
        {
            SetCursorPos(x, y);
        }

        public void RightClick(int x, int y)
        {
            mouse_event(MOUSEEVENTF_RIGHTDOWN | MOUSEEVENTF_RIGHTUP, x, y, 0, 0);
        }
    }
}
