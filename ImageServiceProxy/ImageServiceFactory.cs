using System;
using Abstractions;
using ImageServiceProxy.Azure;

namespace ImageServiceProxy
{
    public static class ImageServiceFactory
    {
        private static IImageService _azureService;
        private static IOpenCVService _opencvService;
        private static ILogger _logger;

        public static IImageService Create(string providerName, string serviceUrl, string serviceKey, IComputer computer, ILogger logger)
        {
            if (string.IsNullOrEmpty(providerName))
            {
                return null;
            }

            _logger = logger;

            if (string.Equals(providerName, "azure", StringComparison.OrdinalIgnoreCase))
            {
                return CreateAzureOCR(serviceUrl, serviceKey, computer, _logger);
            }

            // Create other service here...

            return null;
        }

        private static IImageService CreateAzureOCR(string serviceUrl, string serviceKey, IComputer computer, ILogger logger)
        {
            if (_azureService == null)
            {
                IOCRResultTextFinder textFinder = new AzureRecognizeTextFinder();
                _azureService = new AzureOCRService(computer, textFinder, GetOpenCVService(), serviceUrl, serviceKey, logger);
            }

            return _azureService;
        }

        private static IOpenCVService GetOpenCVService()
        {
            return _opencvService ?? (_opencvService = new OpenCVService());
        }
    }
}
