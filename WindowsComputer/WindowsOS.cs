using CommonContracts;

namespace WindowsComputer
{
    public class WindowsOS : IComputer
    {
        public WindowsOS()
        {
            Screen = new WindowsScreen();
            Mouse = new WindowsMouse();
            Keyboard = new WindowsKeyboard(Mouse);
        }

        public IScreen Screen { get; }

        public IKeyboard Keyboard { get; }

        public IMouse Mouse { get; }
    }
}
