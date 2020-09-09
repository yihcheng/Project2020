using System.Collections.Generic;
using P2020.Abstraction;

namespace P2020.Engine
{
    internal abstract class TestStepForBase
    {
        private readonly IEngineConfig _engineConfig;
        private const string _artifactFolderConfigKey = "ArtifactsFolder";

        public TestStepForBase(IReadOnlyList<ICloudOCRService> ocrServices,
                               IEngineConfig config,
                               IComputer computer,
                               ILogger logger,
                               IOpenCVSUtils openCVService)
        {
            OCRServices = ocrServices;
            _engineConfig = config;
            Computer = computer;
            Logger = logger;
            OpenCVService = openCVService;
        }

        protected string GetArtifactFolderValue()
        {
            return string.IsNullOrEmpty(_engineConfig[_artifactFolderConfigKey]) ? "artifacts" : _engineConfig[_artifactFolderConfigKey];
        }

        public IComputer Computer { get; }

        public ILogger Logger { get; }

        public IReadOnlyList<ICloudOCRService> OCRServices { get; }

        public IOpenCVSUtils OpenCVService { get; }
    }
}
