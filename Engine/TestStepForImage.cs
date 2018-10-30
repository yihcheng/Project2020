using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Abstractions;

namespace Engine
{
    internal class TestStepForImage : TestStepForBase, ITestStepExecutor
    {
        private readonly System.Collections.Generic.IReadOnlyList<ICloudOCRService> _ocrServices;
        private readonly IComputer _computer;
        private readonly ITestStep _step;
        private readonly ILogger _logger;
        private const string _targetKeyword = "image";

        public TestStepForImage(System.Collections.Generic.IReadOnlyList<ICloudOCRService> ocrServices, IComputer computer, ITestStep action, ILogger logger, IEngineConfig config)
            : base(config)
        {
            _ocrServices = ocrServices;
            _computer = computer;
            _step = action;
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

            if (!File.Exists(_step.Search))
            {
                _logger.WriteError($"\"{_step.Search}\" is not found");
                return false;
            }

            string fullScreenFile = $".\\{GetArtifactFolderValue()}\\template-{DateTime.Now.ToString("yyyyMMddHHmmss")}.jpg";
            FileUtility.EnsureParentFolder(fullScreenFile);
            _computer.Screen.SaveFullScreenAsFile(fullScreenFile);

            byte[] searchFileBytes = File.ReadAllBytes(_step.Search);
            byte[] fullScreenFileBytes = File.ReadAllBytes(fullScreenFile);
            (double confidence, int X, int Y)? result = null;

            if (_ocrServices?.Count > 0)
            {
                // TemplateMatch is identical in every image service
                result = _ocrServices[0].OpenCVUtils.TemplateMatch(searchFileBytes, fullScreenFileBytes);
            }

            // reject small confidence
            if (result == null)
            {
                _logger.WriteError($"TemplateMatch result is null");
                return false;
            }

            if (result.Value.confidence < 0.9)
            {
                _logger.WriteError($"templateMatch result confidence is less 0.9 (actual:{result.Value.confidence})");
                return false;
            }

            // check screen area
            bool areaFound = false;

            foreach (ScreenSearchArea area in _step.SearchArea)
            {
                if (_computer.Screen.IsSearchAreaMatch(area, (result.Value.X, result.Value.Y)))
                {
                    areaFound = true;
                }
            }

            if (!areaFound)
            {
                _logger.WriteError("Target is not found in selected search area");
                return false;
            }

            _logger.WriteInfo($"Target is found at locaiton ({result.Value.X},{result.Value.Y})");

            // run action if exists
            if (!string.IsNullOrEmpty(_step.Action))
            {
                ITestActionExecutor actionExecutor = TestActionExecutorGenerator.Generate(_computer,
                                                                                          _step.Action,
                                                                                          _step.ActionArgument,
                                                                                          (result.Value.X, result.Value.Y),
                                                                                          _logger);

                actionExecutor?.Execute();
            }

            // wait if exists
            if (_step.WaitingSecond > 0)
            {
                _logger.WriteInfo($"TestStepForImage waits for {_step.WaitingSecond} seconds");
                await Task.Delay(_step.WaitingSecond * 1000).ConfigureAwait(false);
            }

            return true;
        }
    }
}
