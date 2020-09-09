using P2020.Abstraction;

namespace P2020.Engine
{
    internal static class EngineEnvironment
    {
        private const string _imageFolderFallback = ".\\images\\";
        private const string _ArtifactsFolderFallback = ".\\artifacts\\";
        private const string _imageFolderKey = "ImageFolder";
        private const string _ArtifactsFolderKey = "ArtifactsFolder";

        public static void EnsureEnvironment(IEngineConfig config)
        {
            Ensure(config, _imageFolderKey, _imageFolderFallback);
            Ensure(config, _ArtifactsFolderKey, _ArtifactsFolderFallback);
        }

        private static void Ensure(IEngineConfig config, string key, string fallback)
        {
            string value = config[key];

            if (string.IsNullOrEmpty(value))
            {
                value = fallback;
            }

            FileUtility.EnsureParentFolder(value);
        }
    }
}
