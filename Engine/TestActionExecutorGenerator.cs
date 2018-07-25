using CommonContracts;

namespace Engine
{
    internal static class TestActionExecutorGenerator
    {
        public static ITestActionExecutor Generate(IComputer computer, string action, string actionArg, (int X, int Y) location)
        {
            if (string.IsNullOrEmpty(action))
            {
                return null;
            }

            if (action.StartsWith("mouse."))
            {
                return new MouseAction(computer.Mouse, action, location);
            }
            else if (action.StartsWith("keyboard."))
            {
                return new KeyboardAction(computer.Keyboard, action, actionArg, location);
            }

            return null;
        }
    }
}
