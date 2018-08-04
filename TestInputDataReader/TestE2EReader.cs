using System;
using System.Collections.Generic;
using System.IO;
using Abstractions;
using Newtonsoft.Json.Linq;

namespace TestInputDataReader
{
    public class TestE2EReader : ITestE2EReader
    {
        private readonly ILogger _logger;

        public TestE2EReader(ILogger logger)
        {
            _logger = logger;
        }

        public ITestE2E ReadFile(string file)
        {
            ITestE2E e2e = null;

            try
            {
                string fileContent = File.ReadAllText(file);
                JObject jObj = JObject.Parse(fileContent);

                // TODO : parse should fail if some major element is missing

                JArray jSteps = (JArray)jObj["Steps"];
                IReadOnlyList<ITestStep> steps = ParseTestSteps(jSteps);

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
                _logger.WriteError($"E2E file ({file}) has error on parsing! " + ex);
            }

            return e2e;
        }

        private IReadOnlyList<ITestStep> ParseTestSteps(JArray jActions)
        {
            List<ITestStep> actions = new List<ITestStep>();

            for (int actionId = 0; actionId < jActions.Count; actionId++)
            {
                TestStep action = new TestStep
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


