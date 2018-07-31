using System.Collections.Generic;

namespace CommonContracts
{
    public interface IEngineConfig
    {
        string this[string key] { get; }
        IReadOnlyList<IOCRProviderConfig> GetOCRProviders();
    }
}
