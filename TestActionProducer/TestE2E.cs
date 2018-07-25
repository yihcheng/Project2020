using System.Collections.Generic;
using CommonContracts;

namespace TestActionProducer
{
    public class TestE2E : ITestE2E
    {
        public TestE2E()
        {
        }

        public string FullName { get; set; }

        public string ShortName { get; set; }

        public bool Skip { get; set; }

        public string ProgramToLaunch { get; set; }

        public IList<ITestStep> Steps { get; set; }
    }
}
