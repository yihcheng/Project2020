using System;
using System.IO;
using Abstractions;
using Moq;
using TestInputDataReader;
using Xunit;

namespace UnitTests.TestInputData
{
    public class TestE2ETest
    {
        [Theory]
        [InlineData(".\\TestInputData\\teste2e-1.json", "dummytest1", false, "mspaint.exe", 3)]
        [InlineData(".\\TestInputData\\teste2e-2.json", "dummytest2", true, "mspaint2.exe", 2)]
        public void ParseSuccessTest(string e2eFile,
                              string expectedShortName,
                              bool expectedSkip,
                              string expectedProgramToLaunch,
                              int expectedTextSteps)
        {
            string file = Path.Combine(Environment.CurrentDirectory, e2eFile);
            TestE2EReader reader = new TestE2EReader(CreateLoggerMock().Object);
            ITestE2E e2e = reader.ReadFile(file);

            Assert.Equal(expectedShortName, e2e.ShortName);
            Assert.Equal(expectedSkip, e2e.Skip);
            Assert.Equal(expectedProgramToLaunch, e2e.ProgramToLaunch);
            Assert.NotNull(e2e.Steps);
            Assert.Equal(expectedTextSteps, e2e.Steps.Count);
        }

        private Mock<ILogger> CreateLoggerMock()
        {
            Mock<ILogger> loggerMock = new Mock<ILogger>();
            loggerMock.Setup(m => m.WriteInfo(It.IsAny<string>()));
            loggerMock.Setup(m => m.WriteError(It.IsAny<string>()));

            return loggerMock;
        }

        [Theory]
        [InlineData(".\\TestInputData\\teste2e-environmentvariable.json", "var1", "value1")]
        public void EnvironmentVariableTest(string e2eFile, string environmentName, string expectedValue)
        {
            string originalValue = Environment.GetEnvironmentVariable(environmentName);
            Environment.SetEnvironmentVariable(environmentName, expectedValue);
            string file = Path.Combine(Environment.CurrentDirectory, e2eFile);
            TestE2EReader reader = new TestE2EReader(CreateLoggerMock().Object);
            ITestE2E e2e = reader.ReadFile(file);

            // according to this e2e file, get the first step and get its ActionArgument
            Assert.Equal(expectedValue, e2e.Steps[0].ActionArgument);

            // revert
            Environment.SetEnvironmentVariable(environmentName, originalValue);
        }
    }
}
