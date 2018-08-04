using System.Collections.Generic;

namespace Abstractions
{
    public interface IEngineConfig
    {
        string this[string key] { get; }
        IReadOnlyList<IOCRProviderConfig> GetOCRProviders();
    }
}
