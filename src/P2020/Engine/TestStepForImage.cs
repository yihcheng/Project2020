using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using P2020.Abstraction;

namespace P2020.Engine
{
    internal class TestStepForImage : TestStepForBase, ITestStepExecutor
    {
        private readonly ITestStep _step;
        private const string _targetKeyword = "image";

        public TestStepForImage(IReadOnlyList<ICloudOCRService> ocrServices,
                                IComputer computer,
                                ITestStep action,
                                ILogger logger,
                                IEngineConfig config,
                                IOpenCVSUtils openCVService)
            : base(ocrServices, config, computer, logger, openCVService)
        {
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
                Logger.WriteError(string.Format(EngineResource.SearchTextNotFound, _step.Search));
                return false;
            }

            string fullScreenFile = $".\\{GetArtifactFolderValue()}\\FullScreen-{DateTime.Now.ToString("yyyyMMddHHmmss")}.jpg";
            FileUtility.EnsureParentFolder(fullScreenFile);
            Computer.Screen.SaveFullScreenAsFile(fullScreenFile);

            byte[] searchFileBytes = File.ReadAllBytes(_step.Search);
            byte[] fullScreenFileBytes = File.ReadAllBytes(fullScreenFile);
            (double Confidence, int X, int Y, int Width, int Height)? templateMatchResult = null;

            if (OCRServices?.Count > 0)
            {
                templateMatchResult = OpenCVService.TemplateMatch(searchFileBytes, fullScreenFileBytes);
            }

            // reject small confidence
            if (templateMatchResult == null)
            {
                Logger.WriteError(EngineResource.TemplateMatchNullResult);
                return false;
            }

            if (templateMatchResult.Value.Confidence < 0.9)
            {
                Logger.WriteError(string.Format(EngineResource.TemplateMathResultIsLow, templateMatchResult.Value.Confidence));
                return false;
            }

            // check screen area
            bool areaFound = false;

            foreach (ScreenSearchArea area in _step.SearchArea)
            {
                int centerX = templateMatchResult.Value.X + templateMatchResult.Value.Width / 2;
                int centerY = templateMatchResult.Value.Y + templateMatchResult.Value.Height / 2;

                if (Computer.Screen.IsSearchAreaMatch(area, (centerX, centerY)))
                {
                    areaFound = true;
                }
            }

            if (!areaFound)
            {
                Logger.WriteError(EngineResource.TargetNotFoundInSearchArea);
                return false;
            }

            Logger.WriteInfo(string.Format(EngineResource.TargetNotFoundInLocation, templateMatchResult.Value.X, templateMatchResult.Value.Y));

            // draw a rectangle
            OpenCVService.DrawRedRectangle(fullScreenFile,
                                           templateMatchResult.Value.X,
                                           templateMatchResult.Value.Y,
                                           templateMatchResult.Value.Width,
                                           templateMatchResult.Value.Height);

            // run action if exists
            if (!string.IsNullOrEmpty(_step.Action))
            {
                ITestActionExecutor actionExecutor = TestActionExecutorGenerator.Generate(Computer,
                                                                                          _step.Action,
                                                                                          _step.ActionArgument,
                                                                                          (templateMatchResult.Value.X, templateMatchResult.Value.Y),
                                                                                          Logger);

                actionExecutor?.Execute();
            }

            // wait if exists
            if (_step.WaitingSecond > 0)
            {
                Logger.WriteInfo(string.Format(EngineResource.ImageStepWaitMessage, _step.WaitingSecond));
                await Task.Delay(_step.WaitingSecond * 1000).ConfigureAwait(false);
            }

            return true;
        }
    }
}
