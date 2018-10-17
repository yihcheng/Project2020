using System;
using System.Collections.Generic;
using System.Linq;
using Abstractions;
using Newtonsoft.Json.Linq;

namespace ImageServiceProxy.Azure
{
    internal class AzureRecognizeTextFinder : IOCRResultTextFinder
    {
        public AzureRecognizeTextFinder()
        {
        }

        private IAzureRecognizeTextResponse Deserialize(string azureRecognizeResponseText)
        {
            JObject jObj = JObject.Parse(azureRecognizeResponseText);
            AzureRecognizeTextResponse textResponse = new AzureRecognizeTextResponse
            {
                // status
                Status = (string)jObj["status"]
            };

            AzureRecognitionResult recognitionResult = new AzureRecognitionResult();
            List<IAzureLine> lines = new List<IAzureLine>();
            JArray jsonLines = (JArray)jObj["recognitionResult"]["lines"];

            for (int lineId = 0; lineId < jsonLines.Count; lineId++)
            {
                IAzureLine line = new AzureLine
                {
                    BoundingBox = ParseBoundingBox((JArray)jsonLines[lineId]["boundingBox"]),
                    Text = (string)jsonLines[lineId]["text"],
                    Words = ParseWord((JArray)jsonLines[lineId]["words"])
                };
                lines.Add(line);
            }

            recognitionResult.Lines = lines.ToArray();
            textResponse.RecognitionResult = recognitionResult;

            return textResponse;
        }

        private IAzureWord[] ParseWord(JArray word)
        {
            List<IAzureWord> words = new List<IAzureWord>();

            for (int id = 0; id < word.Count; id++)
            {
                IAzureWord w = new AzureWord
                {
                    Text = (string)word[id]["text"],
                    BoundingBox = ParseBoundingBox((JArray)word[id]["boundingBox"])
                };
                words.Add(w);
            }

            return words.ToArray();
        }

        private int[] ParseBoundingBox(JArray boudingBox)
        {
            int[] points = new int[8];

            for(int id =0; id <8;id++)
            {
                points[id] = (int)boudingBox[id];
            }

            return points;
        }

        public bool TrySearchText(string textToSearch,
                                  string jsonResult,
                                  IScreen screen,
                                  ScreenSearchArea searchArea,
                                  out IScreenArea area)
        {
            if (string.IsNullOrEmpty(textToSearch))
            {
                throw new ArgumentNullException();
            }

            area = null;
            IAzureRecognizeTextResponse textResponse = Deserialize(jsonResult);

            if (textResponse.RecognitionResult?.Lines?.Length == 0)
            {
                return false;
            }

            bool isLine = textToSearch.Trim().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Length > 1;

            foreach (IAzureLine line in textResponse.RecognitionResult?.Lines)
            {
                (int X, int Y) = line.GetArea().GetCentralPoint();

                if (isLine)
                {
                    if (line.Text.IndexOf(textToSearch, StringComparison.OrdinalIgnoreCase) >= 0
                        && screen.IsSearchAreaMatch(searchArea, (X, Y)))
                    {
                        area = line.GetArea();
                        return true;
                    }
                }
                else if (line.Words.Length > 0)
                {
                    IAzureWord word = Array.Find(line.Words, x => string.Equals(x.Text, textToSearch, StringComparison.OrdinalIgnoreCase));

                    if (word != null)
                    {
                        (int X2, int Y2) = word.GetArea().GetCentralPoint();

                        if (screen.IsSearchAreaMatch(searchArea, (X2, Y2)))
                        {
                            area = word.GetArea();
                            return true;
                        }
                    }
                }
            }

            return false;
        }
    }
}
