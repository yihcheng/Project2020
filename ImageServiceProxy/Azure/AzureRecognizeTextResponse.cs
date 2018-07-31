using CommonContracts;

namespace ImageServiceProxy.Azure
{
    internal class AzureRecognizeTextResponse : IAzureRecognizeTextResponse
    {
        public string Status { get; set; }
        public IAzureRecognizeResult RecognitionResult { get; set; }
    }

    internal class RecognitionResult : IAzureRecognizeResult
    {
        public IAzureLine[] Lines { get; set; }
    }

    internal class Line : IAzureLine
    {
        public int[] BoundingBox { get; set; }
        public string Text { get; set; }
        public IAzureWord[] Words { get; set; }

        public IScreenLocation GetCentralLocation()
        {
            return LocationUtility.GetCentralLocation(BoundingBox);
        }
    }

    internal class Word : IAzureWord
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


