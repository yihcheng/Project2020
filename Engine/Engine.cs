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

        public async Task RunAsync(string e2eFile = null)
        {
            string[] e2eFiles = GetAllTestE2EFiles();
            bool finalResult = true;

            for (int i = 0; i < e2eFiles.Length; i++)
            {
                if (!string.IsNullOrEmpty(e2eFile)
                    && !string.Equals(Path.GetFileName(e2eFiles[i]), e2eFile, StringComparison.OrdinalIgnoreCase))
                {
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

        private static string[] GetAllTestE2EFiles()
        {
            string folder = Path.Combine(Environment.CurrentDirectory, _engineConfig["TestJsonFileFolder"]);

            return Directory.GetFiles(folder, "*.json");
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
