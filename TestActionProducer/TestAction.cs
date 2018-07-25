using CommonContracts;

namespace TestActionProducer
{
    public class TestAction : ITestStep
    {
        public TestAction()
        {
        }

        public string Target { get; set; }

        public string Search { get; set; }

        public string Action { get; set; }

        public string ActionArgument { get; set; }

        public ScreenSearchArea SearchArea { get; set; }

        public int WaitingSecond { get; set; }

        public bool FailureReport { get; set; }
    }
}
