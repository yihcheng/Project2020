using System;
using CommonContracts;
using ImageServiceProxy.Azure;

namespace ImageServiceProxy
{
    public static class ImageServiceFactory
    {
        private static IImageService _azureService;
        private static IOpenCVService _opencvService;

        public static IImageService Create(string providerName, string serviceUrl, string serviceKey, IComputer computer)
        {
            if (string.IsNullOrEmpty(providerName))
            {
                return null;
            }

            if (string.Equals(providerName, "azure", StringComparison.OrdinalIgnoreCase))
            {
                return CreateAzureOCR(serviceUrl, serviceKey, computer);
            }

            return null;
        }

        private static IImageService CreateAzureOCR(string serviceUrl, string serviceKey, IComputer computer)
        {
            if (_azureService == null)
            {
                IOCRResultTextFinder textFinder = new AzureRecognizeTextFinder();
                _azureService = new AzureOCRService(computer, textFinder, GetOpenCVService(), serviceUrl, serviceKey);
            }

            return _azureService;
        }

        private static IOpenCVService GetOpenCVService()
        {
            return _opencvService ?? (_opencvService = new OpenCVService());
        }
    }
}
