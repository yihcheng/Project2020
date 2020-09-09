using System;
using System.Threading;
using P2020.Abstraction;

namespace P2020.Engine
{
    internal class MouseAction : ITestActionExecutor
    {
        private readonly IMouse _mouse;
        private readonly string _action;
        private readonly (int X, int Y) _location;
        private readonly ILogger _logger;
        private readonly int _screenWidth;
        private readonly int _screenHeight;

        public MouseAction(IMouse mouse, string action, (int X, int Y) location, ILogger logger, int width, int height)
        {
            _mouse = mouse;
            _action = action;
            _location = location;
            _logger = logger;
            _screenWidth = width;
            _screenHeight = height;
        }

        public void Execute()
        {
            string[] actionData = _action.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

            if (actionData.Length != 2)
            {
                _logger.WriteError("Action is not in a good format");
                return;
            }

            _logger.WriteInfo($"Mouse command = {actionData[1]} in location ({_location.X},{_location.Y})");
            DoMouseAction(actionData[1], _location);
        }

        private void DoMouseAction(string command, (int X, int Y) location)
        {
            command = command.ToLower();

            switch (command)
            {
                case "click":
                    _mouse.MoveTo(location.X, location.Y);
                    Thread.Sleep(500);
                    _mouse.Click(location.X, location.Y);
                    break;
                case "doubleclick":
                    _mouse.MoveTo(location.X, location.Y);
                    Thread.Sleep(500);
                    _mouse.DoubleClick(location.X, location.Y);
                    break;
                case "moveto":
                    _mouse.MoveTo(location.X, location.Y);
                    break;
                case "rightclick":
                    _mouse.MoveTo(location.X, location.Y);
                    Thread.Sleep(500);
                    _mouse.RightClick(location.X, location.Y);
                    break;
            }

            Thread.Sleep(500);
            _mouse.MoveTo(_screenWidth, _screenHeight);
        }
    }
}
