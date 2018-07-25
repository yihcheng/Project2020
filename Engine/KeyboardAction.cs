using System;
using CommonContracts;

namespace Engine
{
    internal class KeyboardAction : ITestActionExecutor
    {
        private readonly IKeyboard _keyboard;
        private readonly string _action;
        private readonly string _actionArg;
        private readonly (int X, int Y) _location;

        public KeyboardAction(IKeyboard keyboard, string action, string actionArg, (int X, int Y) location)
        {
            _keyboard = keyboard;
            _action = action;
            _actionArg = actionArg;
            _location = location;
        }

        public void Execute()
        {
            string[] actionData = _action.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

            if (actionData.Length != 2)
            {
                // TODO: log?
                Console.WriteLine("Action is not in a good format");
                return;
            }
        
            // TODO: log?
            Console.WriteLine($"Keyboard command = {actionData[1]}, content = {_actionArg} in location ({_location.X},{_location.Y})");
            DoKeyboardAction(actionData[1], _location);
        }

        private void DoKeyboardAction(string command, (int X, int Y) location)
        {
            command = command.ToLower();

            if (string.Equals(command, "sendkeys", StringComparison.OrdinalIgnoreCase))
            {
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
