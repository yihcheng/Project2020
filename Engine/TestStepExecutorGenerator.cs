using System;
using CommonContracts;

namespace Engine
{
    internal static class TestStepExecutorGenerator
    {
        public static ITestStepExecutor Generate(ITestStep testStep, IComputer computer, IImageService imageSerivce)
        {
            if (string.Equals(testStep.Target, "image", StringComparison.OrdinalIgnoreCase))
            {
                return new TestStepForImage(imageSerivce, computer, testStep);
            }
            else if (string.Equals(testStep.Target, "text", StringComparison.OrdinalIgnoreCase))
            {
                // this is v1
                // return new TestStepForText(imageSerivce, computer, testStep);
                // this is v2
                return new TestStepForText2(imageSerivce, computer, testStep);
            }
            else if (string.Equals(testStep.Target, "", StringComparison.OrdinalIgnoreCase))
            {
                return new TestStepForActionOnly(computer, testStep);
            }

            return null;
        }
    }
}
