using System.Collections.Generic;
using P2020.Abstraction;

namespace P2020.Engine
{
    internal static class CloudOCRServiceProvider
    {
        public static IReadOnlyList<ICloudOCRService> GetCloudOCRServices(IComputer computer, IEngineConfig config, ILogger logger)
        {
            IReadOnlyList<IOCRProviderConfig> ocrProviders = config.GetOCRProviders();

            if (ocrProviders == null || ocrProviders.Count == 0)
            {
                return null;
            }

            List<ICloudOCRService> services = new List<ICloudOCRService>();

            foreach (IOCRProviderConfig providerConfig in ocrProviders)
            {
                ICloudOCRService service = CloudOCRServiceFactory.Create(providerConfig.Provider, providerConfig.Endpoint, providerConfig.Key, computer, logger, config);

                if (service != null)
                {
                    services.Add(service);
                }
            }

            return services;
        }
    }
}
