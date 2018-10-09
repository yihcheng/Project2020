using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Abstractions;

namespace Engine
{
    internal class Engine : IEngine
    {
        // TODO: should not hard-code it. It should go to config file.
        private static ITestE2EReader _testE2EReader;
        private static IComputer _computer;
        private static IReadOnlyList<IImageService> _imageServices;
        private static IEngineConfig _engineConfig;
        private static ILogger _logger;

        public Engine(ITestE2EReader reader, IComputer computer, IReadOnlyList<IImageService> services, IEngineConfig config, ILogger logger)
        {
            _testE2EReader = reader;
            _computer = computer;
            _imageServices = services;
            _engineConfig = config;
            _logger = logger;
            // ensure environment
            EngineEnvironment.EnsureEnvironment(config);
        }

        public async Task RunAsync(string[] e2eFiles = null)
        {
            if (e2eFiles == null || e2eFiles.Length == 0)
            {
                _logger.WriteInfo(EngineResource.NoInputFile);
                _logger.WriteInfo("");
                _logger.WriteInfo(EngineResource.EngineMessage);
                return;
            }

            bool finalResult = true;

            for (int i = 0; i < e2eFiles.Length; i++)
            {
                if (string.IsNullOrEmpty(e2eFiles[i]))
                {
                    continue;
                }

                if (!File.Exists(e2eFiles[i]))
                {
                    _logger.WriteInfo($"{e2eFiles[i]} doesn't exist.");
                    continue;
                }

                try
                {
                    ITestE2E teste2e = _testE2EReader.ReadFile(e2eFiles[i]);

                    if (teste2e.Skip)
                    {
                        continue;
                    }

                    _logger.WriteInfo($"Running E2E test - {teste2e.FullName}");

                    TestE2EExecutor executor = new TestE2EExecutor(teste2e, _computer, _imageServices, _logger);
                    bool result = await executor.ExecuteAsync().ConfigureAwait(false);
                    finalResult = finalResult && result;

                    _logger.WriteInfo($"{teste2e.FullName} is done. The result is {result}");
                    await Task.Delay(1000).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    _logger.WriteError("Engine run error message = " + ex);
                }
            }

            DisplayFinalResult(finalResult);
        }

        private void DisplayFinalResult(bool finalResult)
        {
            if (finalResult)
            {
                _computer.DisplayMessageBox("The final test result is PASS!");
            }
            else
            {
                _computer.DisplayMessageBox("The final test result is FAILED!");
            }
        }
    }
}
