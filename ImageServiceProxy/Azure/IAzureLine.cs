using Abstractions;

namespace ImageServiceProxy.Azure
{
    internal interface IAzureLine
    {
        int[] BoundingBox { get; set; }
        string Text { get; set; }
        IAzureWord[] Words { get; set; }
        IScreenArea GetArea();
    }
}
