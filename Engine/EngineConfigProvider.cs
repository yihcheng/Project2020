using CommonContracts;

namespace Engine
{
    internal static class EngineConfigProvider
    {
        private static string configFilePath = ".\\EngineConfig.json";

        public static IEngineConfig GetConfig()
        {
            return new EngineConfig(configFilePath);
        }
    }
}
