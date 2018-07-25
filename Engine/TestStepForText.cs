using System;
using System.IO;
using System.Threading.Tasks;
using CommonContracts;
using ImageServiceProxy;

namespace Engine
{
    internal class TestStepForText : ITestStepExecutor
    {
        private readonly IImageService _service;
        private readonly IComputer _computer;
        private readonly ITestStep _step;
        private const string _targetKeyword = "text";

        public TestStepForText(IImageService service, IComputer computer, ITestStep step)
        {
            _service = service;
            _computer = computer;
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

            string filename = $".\\ScreenShotTemplate\\fullscreen-{DateTime.Now.ToString("yyyyMMddHHmmss")}.jpg";
            FileUtility.CreateParentFolder(filename);
            _computer.Screen.SaveFullScreenAsFile(filename);
            string ocrString = await _service.AzureOCR(filename).ConfigureAwait(false);
            bool isLine = _step.Search.Trim().Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries).Length > 1;
            AzureOCRTextFinder textFinder = new AzureOCRTextFinder(ocrString, _computer.Screen, isLine);

            if (!textFinder.TrySearchText(_step.Search, _step.SearchArea, out IScreenLocation location))
            {
                string ocrFailFileName = $"ACRfail{DateTime.Now.ToString("yyyyMMddHHmmss")}-{_step.Search}.txt";
                string ocrFailFilePath = $".\\OCRfail\\{ocrFailFileName}";
                FileUtility.CreateParentFolder(ocrFailFilePath);
                File.WriteAllText(ocrFailFilePath, ocrString);
                // TODO: log?
                Console.WriteLine($"AzureOCR didn't find text - {_step.Search}. {ocrFailFileName} is saved");
                return false;
            }

            Console.WriteLine($"{_step.Search} is found at location ({location.X}, {location.Y})");

            // run action if exists
            if (!string.IsNullOrEmpty(_step.Action))
            {
                ITestActionExecutor actionExecutor = TestActionExecutorGenerator.Generate(_computer,
                                                                                          _step.Action,
                                                                                          _step.ActionArgument,
                                                                                          (location.X, location.Y));

                actionExecutor?.Execute();
            }

            // wait if exists
            if (_step.WaitingSecond > 0)
            {
                await Task.Delay(_step.WaitingSecond * 1000).ConfigureAwait(false);
            }

            return true;
        }
    }
}
