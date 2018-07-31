using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using CommonContracts;

namespace Engine
{
    internal class TestE2EExecutor
    {
        private readonly ITestE2E _testE2E;
        private readonly IComputer _computer;
        private readonly IReadOnlyList<IImageService> _imageService;
        private Process _targetProcess;

        public TestE2EExecutor(ITestE2E testE2E, IComputer computer, IReadOnlyList<IImageService> imageService)
        {
            _testE2E = testE2E;
            _computer = computer;
            _imageService = imageService;
        }

        public async Task<bool> ExecuteAsync()
        {
            if (_targetProcess == null)
            {
                LaunchTargetProgram();
            }

            if (_testE2E.Steps == null
                || _testE2E.Steps.Count == 0)
            {
                return false;
            }

            bool finalResult = true;

            foreach (ITestStep step in _testE2E.Steps)
            {
                ITestStepExecutor testStepExecutor = TestStepExecutorGenerator.Generate(step, _computer, _imageService);

                if (testStepExecutor == null)
                {
                    // TODO: log?
                    continue;
                }

                bool result = await testStepExecutor.ExecuteAsync().ConfigureAwait(false);

                if (step.FailureReport)
                {
                    // TODO: log?
                    Console.WriteLine($"failResult = {finalResult}");
                    finalResult = finalResult && result;
                }

                if (!result && step.FailureReport)
                {
                    break;
                }

                // TODO: log result?

            }

            CloseTargetProgram();

            return finalResult;
        }

        private void CloseTargetProgram()
        {
            if (_targetProcess?.HasExited == false)
            {
                try
                {
                    _targetProcess.Kill();
                }
                catch
                {
                    // TODO: log?
                }
            }
        }

        private void LaunchTargetProgram()
        {
            // some program may launch in test steps
            if (string.IsNullOrEmpty(_testE2E.ProgramToLaunch))
            {
                return;
            }

            _targetProcess = Process.Start(_testE2E.ProgramToLaunch);
            // wait for 10 seconds and let target program be ready
            // It's stupid to wait for a fixed amount of time. 
            // TODO:We should make it defined in json.
            Thread.Sleep(10000);
        }
    }
}
