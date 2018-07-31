using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CommonContracts;

namespace Engine
{
    internal class TestStepForImage : ITestStepExecutor
    {
        private readonly IReadOnlyList<IImageService> _services;
        private readonly IComputer _computer;
        private readonly ITestStep _step;
        private readonly string _targetKeyword = "image";

        public TestStepForImage(IReadOnlyList<IImageService> services, IComputer computer, ITestStep action)
        {
            _services = services;
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
                // TODO : log?
                Console.WriteLine($"{_step.Search} is not found");
                return false;
            }

            string fullScreenFile = $".\\ScreenShotTemplate\\template-{DateTime.Now.ToString("yyyyMMddHHmmss")}.jpg";
            // TODO
            FileUtility.CreateParentFolder(fullScreenFile);
            _computer.Screen.SaveFullScreenAsFile(fullScreenFile);

            byte[] searchFileBytes = File.ReadAllBytes(_step.Search);
            byte[] fullScreenFileBytes = File.ReadAllBytes(fullScreenFile);
            (double confidence, int X, int Y)? result = null;

            if (_services?.Count > 0)
            {
                // TemplateMatch is identical in every image service
                result = _services[0].TemplateMatch(searchFileBytes, fullScreenFileBytes);
            }

            // reject small confidence
            if (result == null)
            {
                // TODO : log?
                Console.WriteLine($"templateMatchResult is null");
                return false;
            }

            if (result.Value.confidence < 0.9)
            {
                // TODO : log?
                Console.WriteLine($"templateMatchResult confidence is less 0.9 (actual:{result.Value.confidence})");
                return false;
            }

            // check screen area
            if (!_computer.Screen.IsSearchAreaMatch(_step.SearchArea, (result.Value.X, result.Value.Y)))
            {
                // TODO : log?
                Console.WriteLine("Target is not found in selected search area");
                return false;
            }

            //TODO: log?
            Console.WriteLine($"Target is found at locaiton ({result.Value.X},{result.Value.Y})");

            // run action if exists
            if (!string.IsNullOrEmpty(_step.Action))
            {
                ITestActionExecutor actionExecutor = TestActionExecutorGenerator.Generate(_computer,
                                                                                          _step.Action,
                                                                                          _step.ActionArgument,
                                                                                          (result.Value.X, result.Value.Y));

                actionExecutor?.Execute();
            }

            // wait if exists
            if (_step.WaitingSecond > 0)
            {
                // TODO: log?
                Console.WriteLine($"TestStepForImage waits for {_step.WaitingSecond} seconds");
                await Task.Delay(_step.WaitingSecond * 1000).ConfigureAwait(false);
            }

            return true;
        }
    }
}
