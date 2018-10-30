using System;
using System.Collections.Generic;
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
        [InlineData(".\\TestInputData\\teste2e-1.json", "dummytest1", false, "mspaint.exe", 5)]
        [InlineData(".\\TestInputData\\teste2e-2.json", "dummytest2", true, "mspaint2.exe", 2)]
        public void ParseSuccessTest(string e2eFile,
                                     string expectedShortName,
                                     bool expectedSkip,
                                     string expectedProgramToLaunch,
                                     int expectedTextSteps)
        {
            ITestE2E e2e = CreateTestE2E(e2eFile);

            Assert.Equal(expectedShortName, e2e.ShortName);
            Assert.Equal(expectedSkip, e2e.Skip);
            Assert.Equal(expectedProgramToLaunch, e2e.ProgramToLaunch);
            Assert.NotNull(e2e.Steps);
            Assert.Equal(expectedTextSteps, e2e.Steps.Count);
        }

        [Theory]
        [InlineData(".\\TestInputData\\teste2e-1.json", 2)]
        [InlineData(".\\TestInputData\\teste2e-2.json", 0)]
        public void RetryAttributeParseTest(string e2eFile, int expectedRetry)
        {
            ITestE2E e2e = CreateTestE2E(e2eFile);

            Assert.NotEmpty(e2e.Steps);
            Assert.Equal(expectedRetry, e2e.Steps[0].Retry);
        }

        private ITestE2E CreateTestE2E(string e2eFile)
        {
            string file = Path.Combine(Environment.CurrentDirectory, e2eFile);
            TestE2EReader reader = new TestE2EReader(CreateLoggerMock().Object);

            return reader.ReadFile(file);
        }

        [Theory]
        [InlineData(".\\TestInputData\\teste2e-3.json", 0, true, true)]
        [InlineData(".\\TestInputData\\teste2e-3.json", 1, true, true)]
        public void VerifyWaitingAndRetry(string e2eFile, int step, bool emptyWaitingTime, bool emptyRetry)
        {
            ITestE2E e2e = CreateTestE2E(e2eFile);

            Assert.NotEmpty(e2e.Steps);
            Assert.Equal(emptyWaitingTime, e2e.Steps[step].WaitingSecond == 0);
            Assert.Equal(emptyRetry, e2e.Steps[step].Retry == 0);
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
        [InlineData(".\\TestInputData\\teste2e-1.json", "var1", null)]
        public void EnvironmentVariableTest(string e2eFile, string environmentName, string expectedValue)
        {
            string originalValue = Environment.GetEnvironmentVariable(environmentName);
            Environment.SetEnvironmentVariable(environmentName, expectedValue);
            string file = Path.Combine(Environment.CurrentDirectory, e2eFile);
            TestE2EReader reader = new TestE2EReader(CreateLoggerMock().Object);
            ITestE2E e2e = reader.ReadFile(file);

            // according to this e2e file, get the first step and get its ActionArgument
            Assert.Equal(expectedValue, e2e.Steps[0].ActionArgument);

            // revert environment variable
            Environment.SetEnvironmentVariable(environmentName, originalValue);
        }

        [Theory]
        [InlineData(".\\TestInputData\\teste2e-IsLaunchProgramMaxTrue.json", true)]
        [InlineData(".\\TestInputData\\teste2e-IsLaunchProgramMaxFalse.json", false)]
        [InlineData(".\\TestInputData\\teste2e-1.json", true)]
        public void IsLaunchedProgramMaximizedTest(string e2eFile, bool expectedValue)
        {
            string file = Path.Combine(Environment.CurrentDirectory, e2eFile);
            TestE2EReader reader = new TestE2EReader(CreateLoggerMock().Object);
            ITestE2E e2e = reader.ReadFile(file);

            Assert.Equal(expectedValue, e2e.MakeLaunchedProgramMaximized);
        }

        [Theory]
        [InlineData(".\\TestInputData\\teste2e-NoStep.json", 0)]
        [InlineData(".\\TestInputData\\teste2e-1.json", 5)]
        public void TestStepCountTest(string e2eFile, int expectedCount)
        {
            string file = Path.Combine(Environment.CurrentDirectory, e2eFile);
            TestE2EReader reader = new TestE2EReader(CreateLoggerMock().Object);
            ITestE2E e2e = reader.ReadFile(file);

            Assert.Equal(expectedCount, e2e.Steps.Count);
        }

        [Theory]
        [MemberData(nameof(ScreenAreaTestData))]
        public void SearchScreenAreaTest(string e2eFile, int stepIndex, List<ScreenSearchArea> expectedAreas)
        {
            string file = Path.Combine(Environment.CurrentDirectory, e2eFile);
            TestE2EReader reader = new TestE2EReader(CreateLoggerMock().Object);
            ITestE2E e2e = reader.ReadFile(file);

            IReadOnlyList<ScreenSearchArea> actualAreas = e2e.Steps[stepIndex].SearchArea;
            Assert.Equal(expectedAreas, actualAreas);
        }

        public static IEnumerable<object[]> ScreenAreaTestData =>
           new List<object[]>
           {
               new object[] { ".\\TestInputData\\teste2e-1.json", 0, new List<ScreenSearchArea>() { ScreenSearchArea.Top } },
               new object[] { ".\\TestInputData\\teste2e-1.json", 1, new List<ScreenSearchArea>() { ScreenSearchArea.FullScreen } },
               new object[] { ".\\TestInputData\\teste2e-1.json", 2, new List<ScreenSearchArea>() { ScreenSearchArea.Center } },
               new object[] { ".\\TestInputData\\teste2e-1.json", 3, new List<ScreenSearchArea>() },
               new object[] { ".\\TestInputData\\teste2e-1.json", 4, new List<ScreenSearchArea>() { ScreenSearchArea.Top,
                                                                                                    ScreenSearchArea.Down,
                                                                                                    ScreenSearchArea.DownRight,
                                                                                                    ScreenSearchArea.TopLeft} },
           };
    }
}
