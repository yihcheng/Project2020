using System.Collections.Generic;
using CommonContracts;

namespace ImageServiceProxy
{
    public class AzureOCRResponse : IAzureOCRResponse
    {
        public string Language { get; set; }
        public decimal TextAngle { get; set; }
        public string Orientation { get; set; }
        public IList<IAzureOCRReponseRegion> Regions { get; set; }
    }

    public class AzureOCRReponseRegion : IAzureOCRReponseRegion
    {
        public string BoundingBox { get; set; }
        public IList<IAzureOCRResponseLine> Lines { get; set; }
    }

    public class AzureOCRResponseLine : AzureOCRResponseBoundingBox, IAzureOCRResponseLine
    {
        public IList<IAzureOCRResponseWord> Words { get; set; }

        public string GetWords()
        {
            if (Words?.Count > 0)
            {
                string text = null;
                foreach (AzureOCRResponseWord word in Words)
                {
                    text += $" {word.Text}";
                }

                return text.Trim();
            }

            return null;
        }
    }

    public class AzureOCRResponseWord : AzureOCRResponseBoundingBox, IAzureOCRResponseWord
    {
        public string Text { get; set; }
    }

    public abstract class AzureOCRResponseBoundingBox : IAzureOCRResponseBoundingBox
    {
        public string BoundingBox { get; set; }
        public IScreenLocation GetCentralLocation()
        {
            if (string.IsNullOrEmpty(BoundingBox))
            {
                return null;
            }

            string[] measures = BoundingBox.Split(',');

            if (measures.Length != 4)
            {
                return null;
            }

            if (!int.TryParse(measures[0], out int X)
                || !int.TryParse(measures[1], out int Y)
                || !int.TryParse(measures[2], out int width)
                || !int.TryParse(measures[3], out int height))
            {
                return null;
            }

            return new ScreenLocation(X + (width / 2), Y + (height / 2));
        }
    }
}
