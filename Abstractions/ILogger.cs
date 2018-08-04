namespace Abstractions
{
    public interface ILogger
    {
        void WriteInfo(string message);
        void WriteError(string message);
    }
}
