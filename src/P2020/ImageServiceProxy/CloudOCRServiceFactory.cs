using System;
using P2020.Abstraction;
using P2020.ImageServiceProxy.Azure;

namespace P2020.ImageServiceProxy
{
    public static class CloudOCRServiceFactory
    {
        private static ICloudOCRService _azureService;

        public static ICloudOCRService Create(string providerName, string serviceUrl, string serviceKey, IComputer computer, ILogger logger, IEngineConfig config)
        {
            if (string.IsNullOrEmpty(providerName))
            {
                return null;
            }

            if (string.Equals(providerName, "azure", StringComparison.OrdinalIgnoreCase))
            {
                return CreateAzureOCR(serviceUrl, serviceKey, computer, logger, config);
            }

            // Create other service here...l

            return null;
        }

        private static ICloudOCRService CreateAzureOCR(string serviceUrl, string serviceKey, IComputer computer, ILogger logger, IEngineConfig config)
        {
            if (_azureService == null)
            {
                IOCRResultTextFinder textFinder = new AzureRecognizeTextFinder();
                _azureService = new AzureOCRService(computer, textFinder, serviceUrl, serviceKey, logger, config);
            }

            return _azureService;
        }
    }
}
