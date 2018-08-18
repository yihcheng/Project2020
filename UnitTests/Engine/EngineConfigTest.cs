using System;
using Engine;
using Xunit;

namespace UnitTests.Engine
{
    public class EngineConfigTest
    {
        [Theory]
        [InlineData(".\\Engine\\EngineConfig.json", "key1", "value1")]
        public void EngineOCRProvierKeyEnvVariableTest(string configFile, string environmentName, string expectedValue)
        {
            string originalValue = Environment.GetEnvironmentVariable(environmentName);
            Environment.SetEnvironmentVariable(environmentName, expectedValue);

            EngineConfig engineConfig = new EngineConfig(configFile);
            Assert.Equal(expectedValue, engineConfig.GetOCRProviders()[0].Key);

            // revert
            Environment.SetEnvironmentVariable(environmentName, originalValue);
        }
    }
}
