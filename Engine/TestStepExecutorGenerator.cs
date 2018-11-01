using System;
using System.Collections.Generic;
using Abstractions;
using ImageServiceProxy;

namespace Engine
{
    internal static class TestStepExecutorGenerator
    {
        private static IOpenCVSUtils _opencvService;

        public static ITestStepExecutor Generate(ITestStep testStep,
                                                 IComputer computer,
                                                 IReadOnlyList<ICloudOCRService> ocrSerivces,
                                                 ILogger logger,
                                                 IEngineConfig config)
        {
            if (string.Equals(testStep.Target, "image", StringComparison.OrdinalIgnoreCase))
            {
                return new TestStepForImage(ocrSerivces, computer, testStep, logger, config, OpenCVService);
            }
            else if (string.Equals(testStep.Target, "text", StringComparison.OrdinalIgnoreCase))
            {
                return new TestStepForText(ocrSerivces, computer, testStep, logger, config, OpenCVService);
            }
            else if (string.IsNullOrEmpty(testStep.Target))
            {
                return new TestStepForActionOnly(computer, testStep, logger);
            }

            return null;
        }

        private static IOpenCVSUtils OpenCVService => _opencvService ?? (_opencvService = new OpenCVService());
    }
}
