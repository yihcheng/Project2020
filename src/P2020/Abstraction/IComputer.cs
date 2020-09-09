namespace P2020.Abstraction
{
    public interface IComputer
    {
        IScreen Screen { get; }
        IKeyboard Keyboard { get; }
        IMouse Mouse { get; }
        void DisplayMessageBox(string message);
    }
}
