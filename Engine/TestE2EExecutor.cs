using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Abstractions;

namespace Engine
{
    internal class TestE2EExecutor
    {
        private readonly ITestE2E _testE2E;
        private readonly IComputer _computer;
        private readonly System.Collections.Generic.IReadOnlyList<ICloudOCRService> _ocrService;
        private readonly ILogger _logger;
        private Process _targetProcess;
        private readonly IEngineConfig _engineConfig;

        public TestE2EExecutor(ITestE2E testE2E, IComputer computer, System.Collections.Generic.IReadOnlyList<ICloudOCRService> ocrService, ILogger logger, IEngineConfig engineConfig)
        {
            _testE2E = testE2E;
            _computer = computer;
            _ocrService = ocrService;
            _logger = logger;
            _engineConfig = engineConfig;
        }

        public async Task<bool> ExecuteAsync()
        {
            bool isLaunchedSuccessfully = await LaunchTargetProgramAsync(_testE2E.MakeLaunchedProgramMaximized).ConfigureAwait(false);

            if (isLaunchedSuccessfully)
            {
                _logger.WriteInfo(string.Format(EngineResource.TestProgramLaunched, _testE2E.ProgramToLaunch));
            }
            else
            {
                _logger.WriteInfo(string.Format(EngineResource.TestProgramFailedLaunch, _testE2E.ProgramToLaunch));
                return false;
            }

            if (_testE2E.Steps == null || _testE2E.Steps.Count == 0)
            {
                // No step to run. Consider as true result.
                _logger.WriteInfo(string.Format(EngineResource.NoTestStep, _testE2E.FullName));
                return true;
            }

            bool finalResult = true;
            _logger.WriteInfo(string.Format(EngineResource.TestStepCountMessage, _testE2E.Steps.Count));

            foreach (ITestStep step in _testE2E.Steps)
            {
                ITestStepExecutor testStepExecutor = TestStepExecutorGenerator.Generate(step, _computer, _ocrService, _logger, _engineConfig);

                if (testStepExecutor == null)
                {
                    continue;
                }

                int retry = 0;
                bool stepResult = false;

                while (retry <= step.Retry)
                {
                    stepResult = await testStepExecutor.ExecuteAsync().ConfigureAwait(false);

                    if (stepResult)
                    {
                        break;
                    }

                    retry++;
                }

                if (step.FailureReport)
                {
                    finalResult &= stepResult;
                }

                if (!stepResult && step.FailureReport)
                {
                    break;
                }
            }

            CloseTargetProgram();
            _logger.WriteInfo(string.Format(EngineResource.TestProgramClosed, _testE2E.ProgramToLaunch));

            return finalResult;
        }

        private void CloseTargetProgram()
        {
            if (_targetProcess?.HasExited == true)
            {
                return;
            }

            try
            {
                if (!_targetProcess.CloseMainWindow())
                {
                    _targetProcess.Kill();
                }
            }
            catch (Exception ex)
            {
                _logger.WriteInfo($"{EngineResource.FailClosePrefix}  {ex}");
            }
        }

        private async Task<bool> LaunchTargetProgramAsync(bool openMaximized)
        {
            ProcessStartInfo psInfo = new ProcessStartInfo
            {
                FileName = _testE2E.ProgramToLaunch
            };

            if (openMaximized)
            {
                psInfo.WindowStyle = ProcessWindowStyle.Maximized;
            }

            try
            {
                _targetProcess = Process.Start(psInfo);
                
                // wait until we can see window title on UI
                while (string.IsNullOrEmpty(_targetProcess.MainWindowTitle))
                {
                    await Task.Delay(1000).ConfigureAwait(false);
                    _targetProcess.Refresh();
                }
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}
