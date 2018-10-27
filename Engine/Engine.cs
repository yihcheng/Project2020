using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Abstractions;

namespace Engine
{
    internal class Engine : IEngine
    {
        private static ITestE2EReader _testE2EReader;
        private static IComputer _computer;
        private static IReadOnlyList<ICloudOCRService> _ocrServices;
        private static IEngineConfig _engineConfig;
        private static ILogger _logger;

        public Engine(ITestE2EReader reader, IComputer computer, IReadOnlyList<ICloudOCRService> ocrServices, IEngineConfig engineConfig, ILogger logger)
        {
            _testE2EReader = reader;
            _computer = computer;
            _ocrServices = ocrServices;
            _engineConfig = engineConfig;
            _logger = logger;

            // ensure environment
            EngineEnvironment.EnsureEnvironment(engineConfig);
        }

        public async Task RunAsync(string[] e2eFiles = null)
        {
            if (e2eFiles == null || e2eFiles.Length == 0)
            {
                PrintHelpMessage();
                return;
            }

            bool finalResult = true;

            for (int i = 0; i < e2eFiles.Length; i++)
            {
                if (string.IsNullOrEmpty(e2eFiles[i]) || !File.Exists(e2eFiles[i]))
                {
                    _logger.WriteError(string.Format(EngineResource.FileNotExist, e2eFiles[i]));
                    continue;
                }

                try
                {
                    ITestE2E teste2e = _testE2EReader.ReadFile(e2eFiles[i]);

                    if (teste2e.Skip)
                    {
                        continue;
                    }

                    _logger.WriteInfo(string.Format(EngineResource.RunTestHeaderMessage, teste2e.FullName));

                    TestE2EExecutor executor = new TestE2EExecutor(teste2e, _computer, _ocrServices, _logger, _engineConfig);
                    bool result = await executor.ExecuteAsync().ConfigureAwait(false);
                    finalResult &= result;

                    _logger.WriteInfo(string.Format(EngineResource.RunTestFinalMessage, teste2e.FullName, result));
                }
                catch (Exception ex)
                {
                    _logger.WriteError($"{EngineResource.EngineErrorMessagePrefix} {ex}");
                }
            }

            DisplayFinalResult(finalResult);
        }

        private static void PrintHelpMessage()
        {
            _logger.WriteInfo(EngineResource.NoInputFile);
            _logger.WriteInfo("");
            _logger.WriteInfo(EngineResource.EngineMessage);
        }

        private void DisplayFinalResult(bool finalResult)
        {
            if (finalResult)
            {
                _computer.DisplayMessageBox(EngineResource.FinalResultPass);
            }
            else
            {
                _computer.DisplayMessageBox(EngineResource.FinalResultFailed);
            }
        }
    }
}
