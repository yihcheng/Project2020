using System.Collections.Generic;

namespace P2020.Abstraction
{
    public interface IEngineConfig
    {
        string this[string key] { get; }
        IReadOnlyList<IOCRProviderConfig> GetOCRProviders();
    }
}
