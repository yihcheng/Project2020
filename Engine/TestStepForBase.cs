using Abstractions;

namespace Engine
{
    internal class TestStepForBase
    {
        private readonly IEngineConfig _engineConfig;
        private const string _artifactFolderConfigKey = "ArtifactsFolder";

        public TestStepForBase(IEngineConfig config)
        {
            _engineConfig = config;
        }

        protected string GetArtifactFolderValue()
        {
            return string.IsNullOrEmpty(_engineConfig[_artifactFolderConfigKey]) ? "artifacts" : _engineConfig[_artifactFolderConfigKey];
        }
    }
}
