using Abstractions;

namespace Engine
{
    internal static class EngineLoggerProvider
    {
        public static ILogger GetLogger(IEngineConfig config)
        {
            return new EngineLogger(config);
        }
    }
}
