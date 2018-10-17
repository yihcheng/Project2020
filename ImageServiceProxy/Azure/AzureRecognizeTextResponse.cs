using Abstractions;

namespace ImageServiceProxy.Azure
{
    internal class AzureRecognizeTextResponse : IAzureRecognizeTextResponse
    {
        public string Status { get; set; }
        public IAzureRecognizeResult RecognitionResult { get; set; }
    }

    internal class AzureRecognitionResult : IAzureRecognizeResult
    {
        public IAzureLine[] Lines { get; set; }
    }

    internal class AzureLine : IAzureLine
    {
        public int[] BoundingBox { get; set; }
        public string Text { get; set; }
        public IAzureWord[] Words { get; set; }

        public IScreenArea GetArea()
        {
            return AzureLocationUtility.GetScreenLocation(BoundingBox);
        }
    }

    internal class AzureWord : IAzureWord
    {
        public int[] BoundingBox { get; set; }
        public string Text { get; set; }

        public IScreenArea GetArea()
        {
            return AzureLocationUtility.GetScreenLocation(BoundingBox);
        }
    }

    //      (0,1)       (2,3)
    //       -------------
    //       |           |
    //       -------------
    //      (6,7)       (4,5)

    internal static class AzureLocationUtility
    {
        public static IScreenArea GetScreenLocation(int[] boundingBox)
        {
            if (boundingBox == null || boundingBox.Length != 8)
            {
                return null;
            }

            int top = boundingBox[1] < boundingBox[2] ? boundingBox[1] : boundingBox[2];
            int bottom = boundingBox[7] < boundingBox[5] ? boundingBox[5] : boundingBox[7];
            int left = boundingBox[0] < boundingBox[6] ? boundingBox[0] : boundingBox[6];
            int right = boundingBox[2] < boundingBox[4] ? boundingBox[4] : boundingBox[2];

            return new ScreenArea(top, left, bottom, right);
        }
    }
}


