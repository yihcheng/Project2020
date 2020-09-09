using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
using P2020.Abstraction;

namespace P2020.InputDataReader
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
            TestE2E e2e = null;

            try
            {
                string fileContent = File.ReadAllText(file);
                JObject jObj = JObject.Parse(fileContent);
                string fullName = jObj["FullName"]?.Value<string>();
                string shortName = jObj["ShortName"]?.Value<string>();
                string programToLaunch = jObj["ProgramToLaunch"].Value<string>();

                e2e = new TestE2E(fullName, shortName, programToLaunch);

                const string skipKey = "Skip";
                const string stepKey = "Steps";
                const string isMaxKey = "MakeLaunchedProgramMaximized";

                if (jObj[skipKey] != null)
                {
                    e2e.Skip = jObj[skipKey].Value<bool>();
                }

                if (jObj[stepKey] == null)
                {
                    e2e.Steps = new List<ITestStep>();
                }
                else
                {
                    JArray jSteps = (JArray)jObj[stepKey];
                    e2e.Steps = ParseTestSteps(jSteps);
                }

                if (jObj[isMaxKey] != null)
                {
                    e2e.MakeLaunchedProgramMaximized = jObj[isMaxKey].Value<bool>();
                }
            }
            catch (Exception ex)
            {
                _logger.WriteError($"E2E file ({file}) has error on parsing! " + ex);
            }

            return e2e;
        }

        internal IReadOnlyList<ITestStep> ParseTestSteps(JArray jActions)
        {
            List<ITestStep> steps = new List<ITestStep>();

            for (int actionId = 0; actionId < jActions.Count; actionId++)
            {
                TestStep step = new TestStep
                {
                    Target = jActions[actionId]["Target"].Value<string>(),
                    Search = jActions[actionId]["Search"].Value<string>(),
                    FailureReport = jActions[actionId]["FailureReport"].Value<bool>()
                };

                if (jActions[actionId]["ScreenArea"] == null)
                {
                    step.SearchArea = new List<ScreenSearchArea>();
                }
                else
                {
                    step.SearchArea = GetScreenSearchArea((JArray)jActions[actionId]["ScreenArea"]);
                }

                if (jActions[actionId]["Action"] != null)
                {
                    step.Action = jActions[actionId]["Action"].Value<string>();
                }

                // ActionArgument may be a value from environment variable
                if (jActions[actionId]["ActionArgument"] != null
                    && !string.IsNullOrEmpty(jActions[actionId]["ActionArgument"].Value<string>()))
                {
                    step.ActionArgument = Environment.ExpandEnvironmentVariables(jActions[actionId]["ActionArgument"].Value<string>());
                }

                // it's reasonable to reject a waiting time larger than 1000 seconds
                if (jActions[actionId]["Waiting"] != null
                    && int.TryParse(jActions[actionId]["Waiting"].Value<string>(), out int waiting)
                    && waiting > 0
                    && waiting < 1000)
                {
                    step.WaitingSecond = waiting;
                }

                // it's reasonable to reject a retry number larger than 10
                if (jActions[actionId]["Retry"] != null
                    && int.TryParse(jActions[actionId]["Retry"].Value<string>(), out int retry)
                    && retry > 0
                    && retry < 11)
                {
                    step.Retry = retry;
                }

                steps.Add(step);
            }

            return steps;
        }

        private static IReadOnlyList<ScreenSearchArea> GetScreenSearchArea(JArray inputArray)
        {
            List<ScreenSearchArea> result = new List<ScreenSearchArea>();

            if (inputArray == null || inputArray.Count == 0)
            {
                return result;
            }

            for (int i = 0; i < inputArray.Count; i++)
            {
                result.Add(GetScreenAreaValue(inputArray[i].Value<string>()));
            }

            return result;
        }

        private static ScreenSearchArea GetScreenAreaValue(string input)
        {
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
                case "right-top":
                case "top-right":
                    return ScreenSearchArea.TopRight;
                case "left-top":
                case "top-left":
                    return ScreenSearchArea.TopLeft;
                case "right-down":
                case "down-right":
                    return ScreenSearchArea.DownRight;
                case "left-down":
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


