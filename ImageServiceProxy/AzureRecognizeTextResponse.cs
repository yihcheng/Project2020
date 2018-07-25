using CommonContracts;

namespace ImageServiceProxy
{
    public class AzureRecognizeTextResponse : IAzureRecognizeTextResponse
    {
        public string Status { get; set; }
        public IRecognizeResult RecognitionResult { get; set; }
    }

    public class RecognitionResult : IRecognizeResult
    {
        public ILine[] Lines { get; set; }
    }

    public class Line : ILine
    {
        public int[] BoundingBox { get; set; }
        public string Text { get; set; }
        public IWord[] Words { get; set; }

        public IScreenLocation GetCentralLocation()
        {
            return LocationUtility.GetCentralLocation(BoundingBox);
        }
    }

    public class Word : IWord
    {
        public int[] BoundingBox { get; set; }
        public string Text { get; set; }

        public IScreenLocation GetCentralLocation()
        {
            return LocationUtility.GetCentralLocation(BoundingBox);
        }
    }

    internal static class LocationUtility
    {
        public static IScreenLocation GetCentralLocation(int[] boundingBox)
        {
            if (boundingBox == null
            || boundingBox.Length != 8)
            {
                return null;
            }

            int X = (boundingBox[0] + boundingBox[2]) / 2;
            int Y = (boundingBox[1] + boundingBox[7]) / 2;

            return new ScreenLocation(X, Y);
        }
    }
}


