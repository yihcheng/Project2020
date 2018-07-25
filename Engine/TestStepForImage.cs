using System;
using System.IO;
using System.Threading.Tasks;
using CommonContracts;

namespace Engine
{
    internal class TestStepForImage : ITestStepExecutor
    {
        private readonly IImageService _service;
        private readonly IComputer _computer;
        private readonly ITestStep _step;
        private readonly string _targetKeyword = "image";

        public TestStepForImage(IImageService imageService, IComputer computer, ITestStep action)
        {
            _service = imageService;
            _computer = computer;
            _step = action;
        }

        public async Task<bool> ExecuteAsync()
        {
            if (_step == null
                || !string.Equals(_step.Target, _targetKeyword, StringComparison.OrdinalIgnoreCase)
                || string.IsNullOrEmpty(_step.Search))
            {
                return false;
            }

            if (!File.Exists(_step.Search))
            {
                Console.WriteLine($"{_step.Search} is not found");
                return false;
            }

            string fullScreenFile = $".\\ScreenShotTemplate\\template-{DateTime.Now.ToString("yyyyMMddHHmmss")}.jpg";
            FileUtility.CreateParentFolder(fullScreenFile);
            _computer.Screen.SaveFullScreenAsFile(fullScreenFile);
            ITemplateMatchResult templateMatchResult = await _service.TemplateMatch(_step.Search, fullScreenFile).ConfigureAwait(false);

            // reject small confidence and non-okay status
            if (templateMatchResult.Confidence < 0.9
                || !string.Equals(templateMatchResult.Message, "ok", StringComparison.OrdinalIgnoreCase))
            {
                // TODO : log?
                Console.WriteLine($"confidence is less 0.9 (actual:{templateMatchResult.Confidence}) or returned message is not okay (actual:{templateMatchResult.Message})");
                return false;
            }

            // check screen area
            if (!_computer.Screen.IsSearchAreaMatch(_step.SearchArea, (templateMatchResult.X, templateMatchResult.Y)))
            {
                // TODO : log?
                Console.WriteLine("target is not found in selected search area");
                return false;
            }

            //TODO: log?
            Console.WriteLine($"target is found at locaiton ({templateMatchResult.X},{templateMatchResult.Y})");

            // run action if exists
            if (!string.IsNullOrEmpty(_step.Action))
            {
                ITestActionExecutor actionExecutor = TestActionExecutorGenerator.Generate(_computer,
                                                                                          _step.Action,
                                                                                          _step.ActionArgument,
                                                                                          (templateMatchResult.X, templateMatchResult.Y));

                actionExecutor?.Execute();
            }

            // wait if exists
            if (_step.WaitingSecond > 0)
            {
                Console.WriteLine($"TestStepForImage waits for {_step.WaitingSecond} seconds");
                await Task.Delay(_step.WaitingSecond * 1000).ConfigureAwait(false);
            }

            return true;
        }
    }
}
