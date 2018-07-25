using System;
using System.IO;
using CommonContracts;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Engine
{
    internal class Engine : IEngine
    {
        // TODO: should not hard-code it. It should go to config file.
        private const string _jsonFileFolder = @".\E2Efiles\";
        private static ITestE2EReader _actionReader;
        private static IComputer _computer;
        private static IImageService _imageService;

        public Engine(ITestE2EReader reader, IComputer computer, IImageService service)
        {
            _actionReader = reader;
            _computer = computer;
            _imageService = service;
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
                    ITestE2E teste2e = _actionReader.Parse(e2eFiles[i]);

                    if (teste2e.Skip)
                    {
                        continue;
                    }

                    // TODO : log?
                    Console.WriteLine($"Running E2E test - {teste2e.FullName}");

                    TestE2EExecutor executor = new TestE2EExecutor(teste2e, _computer, _imageService);
                    bool result = await executor.ExecuteAsync().ConfigureAwait(false);
                    finalResult = finalResult && result;

                    // TODO : log?
                    Console.WriteLine($"{teste2e.FullName} is done. The result is {result}");
                    Console.WriteLine();
                    await Task.Delay(1000).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    // TODO: log?
                    Console.WriteLine("Error message = " + ex.ToString());
                }
            }

            DisplayFinalResult(finalResult);
        }

        private static string[] GetAllTestE2EFiles()
        {
            string folder = Path.Combine(Environment.CurrentDirectory, _jsonFileFolder);

            return Directory.GetFiles(folder, "*.json");
        }

        private void DisplayFinalResult(bool finalResult)
        {
            string result = finalResult ? "PASSED" : "Failed";
            MessageBoxIcon icon = finalResult ? MessageBoxIcon.Information : MessageBoxIcon.Error;
            // TODO: should make it top most
            MessageBox.Show($"The final test result is {result}", "Test Result", MessageBoxButtons.OK, icon);
        }
    }
}
