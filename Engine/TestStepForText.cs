using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abstractions;

namespace Engine
{
    internal class TestStepForText : ITestStepExecutor
    {
        private readonly IReadOnlyList<IImageService> _services;
        private readonly IComputer _computer;
        private readonly ITestStep _step;
        private readonly ILogger _logger;
        private const string _targetKeyword = "text";

        public TestStepForText(IReadOnlyList<IImageService> services, IComputer computer, ITestStep step, ILogger logger)
        {
            _services = services;
            _computer = computer;
            _step = step;
            _logger = logger;
        }

        public async Task<bool> ExecuteAsync()
        {
            if (_step == null
                || !string.Equals(_step.Target, _targetKeyword, StringComparison.OrdinalIgnoreCase)
                || string.IsNullOrEmpty(_step.Search))
            {
                return false;
            }

            string filename = $".\\ScreenShotTemplate\\fullscreen-{DateTime.Now.ToString("yyyyMMddHHmmss")}.jpg";
            FileUtility.CreateParentFolder(filename);
            _computer.Screen.SaveFullScreenAsFile(filename);
            bool foundLocation = false;
            IScreenLocation location = null;

            for (int i = 0; i < _services.Count; i++)
            {
                location = await _services[i].GetOCRResultAsync(filename, _step.Search, _step.SearchArea).ConfigureAwait(false);

                if (location != null)
                {
                    foundLocation = true;
                    break;
                }
            }

            if (foundLocation)
            {
                _logger.WriteInfo($"Text ({_step.Search}) is found at location ({location.X}, {location.Y})");
            }
            else
            {
                _logger.WriteError($"Text ({_step.Search}) is NOT found.");
                return false;
            }

            // run action if exists
            if (!string.IsNullOrEmpty(_step.Action))
            {
                ITestActionExecutor actionExecutor = TestActionExecutorGenerator.Generate(_computer,
                                                                                          _step.Action,
                                                                                          _step.ActionArgument,
                                                                                          (location.X, location.Y),
                                                                                          _logger);

                actionExecutor?.Execute();
            }

            // wait if exists
            if (_step.WaitingSecond > 0)
            {
                _logger.WriteInfo($"TestStepForText2 waits for {_step.WaitingSecond} seconds");
                await Task.Delay(_step.WaitingSecond * 1000).ConfigureAwait(false);
            }

            return true;
        }
    }
}
