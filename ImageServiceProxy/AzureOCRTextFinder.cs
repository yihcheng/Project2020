using System;
using System.Collections.Generic;
using System.Linq;
using CommonContracts;
using Newtonsoft.Json.Linq;

namespace ImageServiceProxy
{
    public class AzureOCRTextFinder : IAzureOCRTextFinder
    {
        private readonly List<(string Text, IScreenLocation Loc, int RegionId)> _lines;
        private readonly List<(string Word, IScreenLocation Loc)> _words;
        private readonly Dictionary<int, IList<IAzureOCRResponseLine>> _regionDict;
        private readonly IAzureOCRResponse _ocrJson;
        private readonly bool _searchLineOnly;
        private readonly IScreen _screen;

        public AzureOCRTextFinder(string jsonV1FromAzureOCR, IScreen screen, bool searchLineOnly = true)
        {
            if (string.IsNullOrEmpty(jsonV1FromAzureOCR) || screen == null)
            {
                throw new ArgumentNullException();
            }

            _searchLineOnly = searchLineOnly;
            _screen = screen;
            _regionDict = new Dictionary<int, IList<IAzureOCRResponseLine>>();
            _lines = new List<(string Text, IScreenLocation Loc, int RegionId)>();

            if (!searchLineOnly)
            {
                _words = new List<(string Word, IScreenLocation loc)>();
            }

            try
            {
                _ocrJson = Deserialize(jsonV1FromAzureOCR);
                if (_ocrJson != null)
                {
                    ParseInternal();
                }
            }
            catch
            {
            }
        }

        private IAzureOCRResponse Deserialize(string input)
        {
            JObject jObj = JObject.Parse(input);
            AzureOCRResponse ocrResponse = new AzureOCRResponse();

            // parse Language, TextAngle, Orientation
            ocrResponse.Language = (string)jObj["language"];
            ocrResponse.TextAngle = (decimal)jObj["textAngle"];
            ocrResponse.Orientation = (string)jObj["orientation"];

            // parse regions, lines, words
            IList<IAzureOCRReponseRegion> regions = new List<IAzureOCRReponseRegion>();
            JArray jRegions = (JArray)jObj["regions"];

            for (int regionId = 0; regionId < jRegions.Count; regionId++)
            {
                IAzureOCRReponseRegion region = new AzureOCRReponseRegion();
                region.BoundingBox = (string)jRegions[regionId]["boundingBox"];
                IList<IAzureOCRResponseLine> regionLines = new List<IAzureOCRResponseLine>();
                JArray jLines = (JArray)jRegions[regionId]["lines"];

                for (int lineId = 0; lineId < jLines.Count; lineId++)
                {
                    IAzureOCRResponseLine line = new AzureOCRResponseLine();
                    line.BoundingBox = (string)jLines[lineId]["boundingBox"];
                    IList<IAzureOCRResponseWord> lineWords = new List<IAzureOCRResponseWord>();
                    JArray jWords = (JArray)jLines[lineId]["words"];

                    for (int wordId = 0; wordId < jWords.Count; wordId++)
                    {
                        IAzureOCRResponseWord word = new AzureOCRResponseWord();
                        word.BoundingBox = (string)jWords[wordId]["boundingBox"];
                        word.Text = (string)jWords[wordId]["text"];
                        lineWords.Add(word);
                    }

                    line.Words = lineWords;
                    regionLines.Add(line);
                }

                region.Lines = regionLines;
                regions.Add(region);
            }

            ocrResponse.Regions = regions;
            return ocrResponse;
        }

        private void ParseInternal()
        {
            try
            {
                for (int regionId = 0; regionId < _ocrJson.Regions.Count; regionId++)
                {
                    IList<IAzureOCRResponseLine> lines = new List<IAzureOCRResponseLine>();

                    foreach (IAzureOCRResponseLine line in _ocrJson.Regions[regionId].Lines)
                    {
                        _lines.Add((line.GetWords(), line.GetCentralLocation(), regionId));

                        if (!_searchLineOnly)
                        {
                            foreach (IAzureOCRResponseWord word in line.Words)
                            {
                                _words.Add((word.Text, word.GetCentralLocation()));
                            }
                        }

                        lines.Add(line);
                    }

                    _regionDict.Add(regionId, lines);
                }
            }
            catch
            {
                // TODO: log?
            }
        }

        public bool TrySearchText(string textToSearch, ScreenSearchArea searchArea, out IScreenLocation location)
        {
            if (string.IsNullOrEmpty(textToSearch))
            {
                throw new ArgumentNullException();
            }

            foreach ((string Text, IScreenLocation loc, int RegionId) in _lines)
            {
                if (string.Equals(Text, textToSearch, StringComparison.OrdinalIgnoreCase)
                    && _screen.IsSearchAreaMatch(searchArea,(loc.X, loc.Y)))
                {
                    location = loc;
                    return true;
                }
            }

            if (!_searchLineOnly)
            {
                foreach ((string word, IScreenLocation loc) item in _words)
                {
                    if (string.Equals(item.word, textToSearch, StringComparison.OrdinalIgnoreCase))
                    {
                        location = item.loc;
                        if (_screen.IsSearchAreaMatch(searchArea, (location.X, location.Y)))
                        {
                            return true;
                        }
                    }
                }
            }

            location = null;
            return false;
        }

        public bool TryGetAllLinesInRegion(int regionId, out IList<IAzureOCRResponseLine> value)
        {
            if (_regionDict.TryGetValue(regionId, out IList<IAzureOCRResponseLine> lines))
            {
                // from top to buttom
                value = lines.OrderBy(a => a.GetCentralLocation().X).ToList();
                return true;
            }

            value = null;
            return false;
        }
    }
}
