using System;
using System.Collections.Generic;
using System.Linq;
using CommonContracts;
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

            RecognitionResult recognitionResult = new RecognitionResult();
            List<IAzureLine> lines = new List<IAzureLine>();
            JArray jsonLines = (JArray)jObj["recognitionResult"]["lines"];

            for (int lineId = 0; lineId < jsonLines.Count; lineId++)
            {
                IAzureLine line = new Line
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
                IAzureWord w = new Word
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
                                  out IScreenLocation location)
        {
            if (string.IsNullOrEmpty(textToSearch))
            {
                throw new ArgumentNullException();
            }

            location = null;
            IAzureRecognizeTextResponse textResponse = Deserialize(jsonResult);

            if (textResponse.RecognitionResult?.Lines?.Length == 0)
            {
                return false;
            }

            bool isLine = textToSearch.Trim().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Length > 1;

            foreach (IAzureLine line in textResponse.RecognitionResult?.Lines)
            {
                int X = line.GetCentralLocation().X;
                int Y = line.GetCentralLocation().Y;

                if (isLine)
                {
                    if (line.Text.IndexOf(textToSearch, StringComparison.OrdinalIgnoreCase) >= 0
                    && screen.IsSearchAreaMatch(searchArea, (X, Y)))
                    {
                        location = line.GetCentralLocation();
                        return true;
                    }
                }
                else if (line.Words.Length > 0)
                {
                    IAzureWord word = line.Words.FirstOrDefault(x => string.Equals(x.Text, textToSearch, StringComparison.OrdinalIgnoreCase));

                    if (word != null && screen.IsSearchAreaMatch(searchArea, (word.GetCentralLocation().X, word.GetCentralLocation().Y)))
                    {
                        location = word.GetCentralLocation();
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
