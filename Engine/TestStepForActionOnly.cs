using System;
using System.Threading.Tasks;
using CommonContracts;

namespace Engine
{
    internal class TestStepForActionOnly : ITestStepExecutor
    {
        private readonly IComputer _computer;
        private readonly ITestStep _step;

        public TestStepForActionOnly(IComputer computer, ITestStep step)
        {
            _computer = computer;
            _step = step;
        }

        public async Task<bool> ExecuteAsync()
        {
            // run action if exists
            if (!string.IsNullOrEmpty(_step.Action))
            {
                ITestActionExecutor actionExecutor = TestActionExecutorGenerator.Generate(_computer,
                                                                                          _step.Action,
                                                                                          _step.ActionArgument,
                                                                                          (-1, -1));

                actionExecutor?.Execute();
            }

            // wait if exists
            if (_step.WaitingSecond > 0)
            {
                Console.WriteLine($"TestStepForActionOnly waits for {_step.WaitingSecond} seconds");
                await Task.Delay(_step.WaitingSecond * 1000).ConfigureAwait(false);
            }

            return true;
        }
    }
}
