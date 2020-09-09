using P2020.Abstraction;

namespace P2020.Engine
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
                return new MouseAction(computer.Mouse, action, location, logger, computer.Screen.Width, computer.Screen.Height);
            }

            if (action.StartsWith("keyboard."))
            {
                return new KeyboardAction(computer.Keyboard, action, actionArg, location, logger);
            }

            return null;
        }
    }
}
