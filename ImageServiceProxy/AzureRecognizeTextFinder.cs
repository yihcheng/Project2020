using System;
using System.Collections.Generic;
using System.Linq;
using CommonContracts;
using Newtonsoft.Json.Linq;

namespace ImageServiceProxy
{
    public class AzureRecognizeTextFinder : IAzureRecognizeTextFinder
    {
        private readonly IAzureRecognizeTextResponse _textResponse;
        private readonly IScreen _screen;
        private readonly bool _isLine;

        public AzureRecognizeTextFinder(string azureRecognizeResponseText, IScreen screen, bool isLine = true)
        {
            if (string.IsNullOrEmpty(azureRecognizeResponseText) || screen == null)
            {
                throw new ArgumentNullException();
            }

            _isLine = isLine;
            _screen = screen;
            _textResponse = Deserialize(azureRecognizeResponseText);
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
            List<ILine> lines = new List<ILine>();
            JArray jsonLines = (JArray)jObj["recognitionResult"]["lines"];

            for (int lineId = 0; lineId < jsonLines.Count; lineId++)
            {
                ILine line = new Line
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

        private IWord[] ParseWord(JArray word)
        {
            List<IWord> words = new List<IWord>();

            for (int id = 0; id < word.Count; id++)
            {
                IWord w = new Word
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

        public bool TrySearchText(string textToSearch, ScreenSearchArea searchArea, out IScreenLocation location)
        {
            if (string.IsNullOrEmpty(textToSearch) || _textResponse == null)
            {
                throw new ArgumentNullException();
            }

            location = null;

            if (_textResponse.RecognitionResult?.Lines?.Length == 0)
            {
                return false;
            }

            foreach (ILine line in _textResponse.RecognitionResult?.Lines)
            {
                int X = line.GetCentralLocation().X;
                int Y = line.GetCentralLocation().Y;

                if (_isLine)
                {
                    if (line.Text.IndexOf(textToSearch, StringComparison.OrdinalIgnoreCase) >= 0
                    && _screen.IsSearchAreaMatch(searchArea, (X, Y)))
                    {
                        location = line.GetCentralLocation();
                        return true;
                    }
                }
                else if (line.Words.Length > 0)
                {
                    IWord word = line.Words.FirstOrDefault(x => string.Equals(x.Text, textToSearch, StringComparison.OrdinalIgnoreCase));

                    if (word != null && _screen.IsSearchAreaMatch(searchArea, (word.GetCentralLocation().X, word.GetCentralLocation().Y)))
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
