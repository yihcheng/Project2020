using Abstractions;

namespace Engine
{
    internal static class EngineEnvironment
    {
        private const string _testJsonFileFolderFallback = ".\\E2EFiles\\";
        private const string _imageFolderFallback = ".\\images\\";
        private const string _screenShotFolderFallback = ".\\screenshot\\";
        private const string _ocrFailFolderFallback = ".\\OCR-Fail\\";
        private const string _testJsonFileFolderKey = "TestJsonFileFolder";
        private const string _imageFolderKey = "ImageFolder";
        private const string _screenShotFolderKey = "ScreenShotFolder";
        private const string _ocrFailFolderKey = "OCRFailFolder";

        public static void EnsureEnvironment(IEngineConfig config)
        {
            Ensure(config, _testJsonFileFolderKey, _testJsonFileFolderFallback);
            Ensure(config, _imageFolderKey, _imageFolderFallback);
            Ensure(config, _screenShotFolderKey, _screenShotFolderFallback);
            Ensure(config, _ocrFailFolderKey, _ocrFailFolderFallback);
        }

        private static void Ensure(IEngineConfig config, string key, string fallback)
        {
            string value = config[key];

            if (string.IsNullOrEmpty(value))
            {
                value = fallback;
            }

            FileUtility.CreateParentFolder(value);
        }
    }
}
