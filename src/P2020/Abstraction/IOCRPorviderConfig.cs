namespace P2020.Abstraction
{
    public interface IOCRProviderConfig
    {
        string Provider { get; }
        string Endpoint { get; }
        string Key { get; }
    }
}
