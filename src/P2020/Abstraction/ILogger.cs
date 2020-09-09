namespace P2020.Abstraction
{
    public interface ILogger
    {
        void WriteInfo(string message);
        void WriteError(string message);
    }
}
