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
                    FullName = jObj["FullName"].Value<string>(),
                    ShortName = jObj["ShortName"].Value<string>(),
                    Skip = jObj["Skip"].Value<bool>(),
                    ProgramToLaunch = jObj["ProgramToLaunch"].Value<string>(),
                    Steps = steps
                };
            }
            catch (Exception ex)
            {
                _logger.WriteError($"E2E file ({file}) has error on parsing! " + ex);
            }

            return e2e;
        }

        internal IReadOnlyList<ITestStep> ParseTestSteps(JArray jActions)
        {
            List<ITestStep> actions = new List<ITestStep>();

            for (int actionId = 0; actionId < jActions.Count; actionId++)
            {
                TestStep action = new TestStep
                {
                    Target = jActions[actionId]["Target"].Value<string>(),
                    Search = jActions[actionId]["Search"].Value<string>(),
                    SearchArea = GetScreenSearchArea(jActions[actionId]["ScreenArea"].Value<string>()),
                    FailureReport = jActions[actionId]["FailureReport"].Value<bool>()
                };

                if (jActions[actionId]["Action"] != null)
                {
                    action.Action = jActions[actionId]["Action"].Value<string>();
                }

                // ActionArgument may be a value from environment variable
                if (jActions[actionId]["ActionArgument"] != null 
                    && !string.IsNullOrEmpty(jActions[actionId]["ActionArgument"].Value<string>()))
                {
                    action.ActionArgument = Environment.ExpandEnvironmentVariables(jActions[actionId]["ActionArgument"].Value<string>());
                }

                // it's reasonable to reject a waiting time larger than 1000 seconds
                if (jActions[actionId]["Waiting"] != null 
                    && int.TryParse(jActions[actionId]["Waiting"].Value<string>(), out int waiting)
                    && waiting > 0
                    && waiting < 1000)
                {
                    action.WaitingSecond = waiting;
                }

                // it's reasonable to reject a retry number larger than 10
                if (jActions[actionId]["Retry"] != null 
                    && int.TryParse(jActions[actionId]["Retry"].Value<string>(), out int retry)
                    && retry > 0
                    && retry < 11)
                {
                    action.Retry = retry;
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


