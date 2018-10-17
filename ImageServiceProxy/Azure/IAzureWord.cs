using Abstractions;

namespace ImageServiceProxy.Azure
{
    internal interface IAzureWord
    {
        int[] BoundingBox { get; set; }
        string Text { get; set; }
        IScreenArea GetArea();
    }
}
