using System;
using System.IO;
using Abstractions;
using Moq;
using TestInputDataReader;
using Xunit;

namespace UnitTests
{
    public class TestE2ETest
    {
        [Theory]
        [InlineData("teste2e-1.json", "dummytest1", false)]
        [InlineData("teste2e-2.json", "dummytest2", true)]
        public void ParseTest(string e2eFile, string expectedShortName, bool expectedSkip)
        {
            string file = Path.Combine(Environment.CurrentDirectory, e2eFile);
            TestE2EReader reader = new TestE2EReader(CreateLoggerMock().Object);
            ITestE2E e2e = reader.ReadFile(file);

            Assert.Equal(expectedShortName, e2e.ShortName);
            Assert.Equal(expectedSkip, e2e.Skip);
        }

        private Mock<ILogger> CreateLoggerMock()
        {
            Mock<ILogger> loggerMock = new Mock<ILogger>();
            loggerMock.Setup(m => m.WriteInfo(It.IsAny<string>()));

            return loggerMock;
        }
    }
}
