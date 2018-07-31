using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommonContracts;

namespace Engine
{
    static class Program
    {
        static async Task Main(string[] args)
        {
            //ConsoleUtility.HideWindow();

            try
            {
                // get engine config
                IEngineConfig _engineConfig = EngineConfigProvider.GetConfig();

                // Create objects for current environment
                ITestE2EReader _e2eReader = TestE2EReaderProvider.GetReader();
                IComputer _computer = ComputerSelector.GetCurrentComputer();
                IReadOnlyList<IImageService> _imageService = ImageServiceProvider.GetImageServices(_computer, _engineConfig);

                // create engine
                IEngine engine = new Engine(_e2eReader, _computer, _imageService, _engineConfig);

                string e2eFile = null;

                if (args.Length == 1)
                {
                    e2eFile = args[0];
                }

                // start engine to run tests
                await engine.RunAsync(e2eFile).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                // TODO: log ?
            }
         
            //Console.WriteLine("done");
            //ConsoleUtility.ShowWindow();
        }
    }
}
