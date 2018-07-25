using System;
using System.Threading.Tasks;
using CommonContracts;

namespace Engine
{
    class Program
    {
        static async Task Main(string[] args)
        {
            ConsoleUtility.HideWindow();

            // Create objects for current environment
            ITestE2EReader  _e2eReader = TestE2EReaderProvider.GetReader();
            IComputer _computer = ComputerSelector.GetCurrentComputer();
            IImageService _imageService = ImageServiceProvider.GetImageService();

            // create engine
            IEngine engine = new Engine(_e2eReader, _computer, _imageService);

            string e2eFile = null;

            if (args.Length == 1)
            {
                e2eFile = args[0];
            }

            // start engine to run tests
            await engine.RunAsync(e2eFile).ConfigureAwait(false);

            Console.WriteLine("done");
            ConsoleUtility.ShowWindow();
        }
    }
}
