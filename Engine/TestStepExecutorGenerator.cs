using System;
using System.Collections.Generic;
using CommonContracts;

namespace Engine
{
    internal static class TestStepExecutorGenerator
    {
        public static ITestStepExecutor Generate(ITestStep testStep, IComputer computer, IReadOnlyList<IImageService> imageSerivces)
        {
            if (string.Equals(testStep.Target, "image", StringComparison.OrdinalIgnoreCase))
            {
                return new TestStepForImage(imageSerivces, computer, testStep);
            }
            else if (string.Equals(testStep.Target, "text", StringComparison.OrdinalIgnoreCase))
            {
                // this is v2
                return new TestStepForText2(imageSerivces, computer, testStep);
            }
            else if (string.Equals(testStep.Target, "", StringComparison.OrdinalIgnoreCase))
            {
                return new TestStepForActionOnly(computer, testStep);
            }

            return null;
        }
    }
}
