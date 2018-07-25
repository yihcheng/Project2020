using CommonContracts;
using TestActionProducer;

namespace Engine
{
    internal static class TestE2EReaderProvider
    {
        public static ITestE2EReader GetReader()
        {
            // return an object which can parse teste2e json file and create ITestE2E instance.
            return new TestActionReader();
        }
    }
}
