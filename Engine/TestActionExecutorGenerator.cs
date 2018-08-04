using Abstractions;

namespace Engine
{
    internal static class TestActionExecutorGenerator
    {
        public static ITestActionExecutor Generate(IComputer computer, string action, string actionArg, (int X, int Y) location, ILogger logger)
        {
            if (string.IsNullOrEmpty(action))
            {
                return null;
            }

            if (action.StartsWith("mouse."))
            {
                return new MouseAction(computer.Mouse, action, location, logger);
            }
            else if (action.StartsWith("keyboard."))
            {
                return new KeyboardAction(computer.Keyboard, action, actionArg, location, logger);
            }

            return null;
        }
    }
}
