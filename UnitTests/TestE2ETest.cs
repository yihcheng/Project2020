using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using TestActionProducer;
using CommonContracts;
using System.IO;

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
            TestActionReader reader = new TestActionReader();
            ITestE2E e2e = reader.Parse(file);

            Assert.Equal(expectedShortName, e2e.ShortName);
            Assert.Equal(expectedSkip, e2e.Skip);
        }
    }
}
