using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using P2020.Abstraction;

namespace P2020.Engine
{
    internal class TestStepForText : TestStepForBase, ITestStepExecutor
    {
        private readonly ITestStep _step;
        private const string _targetKeyword = "text";

        public TestStepForText(IReadOnlyList<ICloudOCRService> ocrServices,
                               IComputer computer,
                               ITestStep step,
                               ILogger logger,
                               IEngineConfig config,
                               IOpenCVSUtils openCVService)
            : base(ocrServices, config, computer, logger, openCVService)
        {
            _step = step;
        }

        public async Task<bool> ExecuteAsync()
        {
            if (_step == null
                || !string.Equals(_step.Target, _targetKeyword, StringComparison.OrdinalIgnoreCase)
                || string.IsNullOrEmpty(_step.Search))
            {
                return false;
            }

            string filename = $".\\{GetArtifactFolderValue()}\\FullScreen-{DateTime.Now.ToString("yyyyMMddHHmmss")}.jpg";
            FileUtility.EnsureParentFolder(filename);
            Computer.Screen.SaveFullScreenAsFile(filename);
            bool foundLocation = false;
            IScreenArea area = null;

            for (int i = 0; i < OCRServices.Count; i++)
            {
                area = await OCRServices[i].GetOCRResultAsync(filename, _step.Search, _step.SearchArea).ConfigureAwait(false);

                if (area != null)
                {
                    foundLocation = true;
                    break;
                }
            }

            if (!foundLocation || area == null)
            {
                Logger.WriteError(string.Format(EngineResource.SearchTextNotFound, _step.Search));
                string message = string.Format(EngineResource.TextNotFoundInJsonResult, _step.Search);
                OpenCVService.PutText(filename, 1, Computer.Screen.Height / 2, message);
                return false;
            }

            (int X, int Y) = area.GetCentralPoint();
            Logger.WriteInfo(string.Format(EngineResource.TargetNotFoundInLocation, X, Y));

            // draw a rectangle
            OpenCVService.DrawRedRectangle(filename, area.Left, area.Top, area.Width, area.Height);

            // run action if exists
            if (!string.IsNullOrEmpty(_step.Action))
            {
                ITestActionExecutor actionExecutor = TestActionExecutorGenerator.Generate(Computer,
                                                                                          _step.Action,
                                                                                          _step.ActionArgument,
                                                                                          (X, Y),
                                                                                          Logger);

                actionExecutor?.Execute();
            }

            // wait if exists
            if (_step.WaitingSecond > 0)
            {
                Logger.WriteInfo(string.Format(EngineResource.TextStepWaitMessage, _step.WaitingSecond));
                await Task.Delay(_step.WaitingSecond * 1000).ConfigureAwait(false);
            }

            return true;
        }
    }
}
