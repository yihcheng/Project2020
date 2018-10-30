using System.Collections.Generic;
using Abstractions;

namespace TestInputDataReader
{
    public class TestStep : ITestStep
    {
        public string Target { get; set; }

        public string Search { get; set; }

        public string Action { get; set; }

        public string ActionArgument { get; set; }

        public int WaitingSecond { get; set; }

        public bool FailureReport { get; set; }

        public int Retry { get; set; }

        public IReadOnlyList<ScreenSearchArea> SearchArea { get; set; }
    }
}
