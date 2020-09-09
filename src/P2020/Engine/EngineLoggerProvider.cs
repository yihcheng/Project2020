using P2020.Abstraction;

namespace P2020.Engine
{
    internal static class EngineLoggerProvider
    {
        public static ILogger GetLogger(IEngineConfig config)
        {
            return new EngineLogger(config);
        }
    }
}
