using System.Threading.Tasks;
using P2020.Abstraction;

namespace P2020.Engine
{
    internal class TestStepForActionOnly : ITestStepExecutor
    {
        private readonly IComputer _computer;
        private readonly ITestStep _step;
        private readonly ILogger _logger;

        public TestStepForActionOnly(IComputer computer, ITestStep step, ILogger logger)
        {
            _computer = computer;
            _step = step;
            _logger = logger;
        }

        public async Task<bool> ExecuteAsync()
        {
            // run action if exists
            if (!string.IsNullOrEmpty(_step.Action))
            {
                ITestActionExecutor actionExecutor = TestActionExecutorGenerator.Generate(_computer,
                                                                                          _step.Action,
                                                                                          _step.ActionArgument,
                                                                                          (-1, -1),
                                                                                          _logger);

                actionExecutor?.Execute();
            }

            // wait if exists
            if (_step.WaitingSecond > 0)
            {
                _logger.WriteInfo($"TestStepForActionOnly waits for {_step.WaitingSecond} seconds");
                await Task.Delay(_step.WaitingSecond * 1000).ConfigureAwait(false);
            }

            return true;
        }
    }
}
