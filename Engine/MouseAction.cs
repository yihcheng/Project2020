using System;
using System.Threading;
using CommonContracts;

namespace Engine
{
    internal class MouseAction : ITestActionExecutor
    {
        private readonly IMouse _mouse;
        private readonly string _action;
        private readonly (int X, int Y) _location;

        public MouseAction(IMouse mouse, string action, (int X, int Y) location)
        {
            _mouse = mouse;
            _action = action;
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

            Console.WriteLine($"Mouse command = {actionData[1]} in location ({_location.X},{_location.Y})");
            DoMouseAction(actionData[1], _location);
        }

        private void DoMouseAction(string command, (int X, int Y) location)
        {
            command = command.ToLower();

            switch (command)
            {
                case "click":
                    _mouse.MoveTo(location.X, location.Y);
                    Thread.Sleep(1000);
                    _mouse.Click(location.X, location.Y);
                    break;
                case "doubleclick":
                    _mouse.MoveTo(location.X, location.Y);
                    Thread.Sleep(1000);
                    _mouse.DoubleClick(location.X, location.Y);
                    break;
                case "moveto":
                    _mouse.MoveTo(location.X, location.Y);
                    break;
                case "rightclick":
                    _mouse.MoveTo(location.X, location.Y);
                    Thread.Sleep(1000);
                    _mouse.RightClick(location.X, location.Y);
                    break;
            }

            Thread.Sleep(500);
            // TODO: this should go to down-right corner of screen
            _mouse.MoveTo(0, 200);
        }
    }
}
