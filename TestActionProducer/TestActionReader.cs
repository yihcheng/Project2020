using System;
using System.Collections.Generic;
using System.IO;
using CommonContracts;
using Newtonsoft.Json.Linq;

namespace TestActionProducer
{
    public class TestActionReader : ITestE2EReader
    {
        public ITestE2E Parse(string file)
        {
            ITestE2E e2e = null;

            try
            {
                string fileContent = File.ReadAllText(file);
                JObject jObj = JObject.Parse(fileContent);

                // actions
                JArray jSteps = (JArray)jObj["Steps"];
                IList<ITestStep> steps = ParseSteps(jSteps);

                e2e = new TestE2E()
                {
                    FullName = (string)jObj["FullName"],
                    ShortName = (string)jObj["ShortName"],
                    Skip = (bool)jObj["Skip"],
                    ProgramToLaunch = (string)jObj["ProgramToLaunch"],
                    Steps = steps
                };
            }
            catch (Exception ex)
            {
                // parse error
                // TODO : log ?
                Console.WriteLine("Parse error = " + ex);
                return null;
            }

            return e2e;
        }

        private IList<ITestStep> ParseSteps(JArray jActions)
        {
            List<ITestStep> actions = new List<ITestStep>();

            for (int actionId = 0; actionId < jActions.Count; actionId++)
            {
                TestAction action = new TestAction
                {
                    Target = (string)jActions[actionId]["Target"],
                    Search = (string)jActions[actionId]["Search"],
                    SearchArea = GetScreenSearchArea((string)jActions[actionId]["ScreenArea"]),
                    FailureReport = (bool)jActions[actionId]["FailureReport"]
                };

                if (jActions[actionId]["Action"] != null)
                {
                    action.Action = (string)jActions[actionId]["Action"];
                }

                if (jActions[actionId]["ActionArgument"] != null)
                {
                    action.ActionArgument = (string)jActions[actionId]["ActionArgument"];
                }

                if (jActions[actionId]["Waiting"] != null)
                {
                    action.WaitingSecond = (int)jActions[actionId]["Waiting"];
                }

                actions.Add(action);
            }

            return actions;
        }

        private ScreenSearchArea GetScreenSearchArea(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return ScreenSearchArea.FullScreen;
            }

            input = input.ToLower();

            switch (input)
            {
                case "right":
                    return ScreenSearchArea.Right;
                case "left":
                    return ScreenSearchArea.Left;
                case "top":
                    return ScreenSearchArea.Top;
                case "down":
                    return ScreenSearchArea.Down;
                case "top-right":
                    return ScreenSearchArea.TopRight;
                case "top-left":
                    return ScreenSearchArea.TopLeft;
                case "down-right":
                    return ScreenSearchArea.DownRight;
                case "down-left":
                    return ScreenSearchArea.DownLeft;
                case "center":
                    return ScreenSearchArea.Center;
                default:
                    return ScreenSearchArea.FullScreen;
            }
        }
    }
}


