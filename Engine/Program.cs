using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abstractions;

namespace Engine
{
    static class Program
    {
        static async Task Main(string[] args)
        {
            // get engine config
            IEngineConfig engineConfig = EngineConfigProvider.GetConfig();
            ILogger logger = EngineLoggerProvider.GetLogger(engineConfig);

            try
            {
                // Create objects for current environment
                ITestE2EReader _e2eReader = TestE2EReaderProvider.GetReader(logger);
                IComputer _computer = ComputerSelector.GetCurrentComputer();
                IReadOnlyList<IImageService> _imageService = ImageServiceProvider.GetImageServices(_computer, engineConfig, logger);

                // create engine
                IEngine engine = new Engine(_e2eReader, _computer, _imageService, engineConfig, logger);

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
                logger.WriteInfo("Error = " + ex);
            }
        }
    }
}
