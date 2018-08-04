using System.Collections.Generic;
using Abstractions;
using ImageServiceProxy;

namespace Engine
{
    internal static class ImageServiceProvider
    {
        public static IReadOnlyList<IImageService> GetImageServices(IComputer computer, IEngineConfig config, ILogger logger)
        {
            IReadOnlyList<IOCRProviderConfig> ocrProviders = config.GetOCRProviders();

            if (ocrProviders==null || ocrProviders.Count ==0)
            {
                return null;
            }

            List<IImageService> services = new List<IImageService>();

            foreach (IOCRProviderConfig providerConfig in ocrProviders)
            {
                IImageService service = ImageServiceFactory.Create(providerConfig.Provider, providerConfig.Endpoint, providerConfig.Key, computer, logger);

                if (service != null)
                {
                    services.Add(service);
                }
            }

            return services;
        }
    }
}
