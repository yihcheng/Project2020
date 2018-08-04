using System;
using Abstractions;

namespace Engine
{
    internal class KeyboardAction : ITestActionExecutor
    {
        private readonly IKeyboard _keyboard;
        private readonly string _action;
        private readonly string _actionArg;
        private readonly (int X, int Y) _location;
        private readonly ILogger _logger;

        public KeyboardAction(IKeyboard keyboard, string action, string actionArg, (int X, int Y) location, ILogger logger)
        {
            _keyboard = keyboard;
            _action = action;
            _actionArg = actionArg;
            _location = location;
            _logger = logger;
        }

        public void Execute()
        {
            string[] actionData = _action.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

            if (actionData.Length != 2)
            {
                _logger.WriteError("Action is not in a good format");
                return;
            }

            _logger.WriteInfo($"Keyboard command = {actionData[1]}, content = {_actionArg} in location ({_location.X},{_location.Y})");
            DoKeyboardAction(actionData[1], _location);
        }

        private void DoKeyboardAction(string command, (int X, int Y) location)
        {
            command = command.ToLower();

            if (string.Equals(command, "sendkeys", StringComparison.OrdinalIgnoreCase))
            {
                // TODO : find another way to handle keyboard action without location here
                if (location.X == -1 && location.Y == -1)
                {
                    _keyboard.SendKeys(_actionArg);
                }
                else
                {
                    _keyboard.SendKeys(location.X, location.Y, _actionArg);
                }
            }
        }
    }
}
