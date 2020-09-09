namespace P2020.ImageServiceProxy.Azure
{
    internal interface IAzureWord
    {
        int[] BoundingBox { get; set; }
        string Text { get; set; }
        IScreenArea GetArea();
    }
}
