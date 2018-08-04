using System;
using Abstractions;

namespace Engine
{
    internal class EngineLogger : ILogger
    {
        private readonly string loggerTypeKey = "LoggerType";
        private Action<string, bool> _writeAction;

        public EngineLogger(IEngineConfig config)
        {
            DecideLoggerType(config);
        }

        private void DecideLoggerType(IEngineConfig config)
        {
            if (string.Equals("Console", config[loggerTypeKey], StringComparison.OrdinalIgnoreCase))
            {
                _writeAction = WriteToConsole;
            }

            // TODO : extend to other logger type here.
        }

        public void WriteInfo(string message)
        {
            _writeAction?.Invoke(message, true);
        }

        private void WriteToConsole(string message, bool displayAsInfo)
        {
            if (!displayAsInfo)
            {
                ConsoleColor color = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("Error: ");
                Console.ForegroundColor = color;
            }

            Console.WriteLine(message);
        }

        public void WriteError(string message)
        {
            _writeAction?.Invoke(message, false);
        }
    }
}
