namespace P2020.Abstraction
{
    public interface IKeyboard
    {
        void SendKeys(string keyString);
        void SendKeys(int x, int y, string keyString);
    }
}
