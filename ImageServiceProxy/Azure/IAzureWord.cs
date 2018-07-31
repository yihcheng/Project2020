using CommonContracts;

namespace ImageServiceProxy.Azure
{
    internal interface IAzureWord
    {
        int[] BoundingBox { get; set; }
        string Text { get; set; }
        IScreenLocation GetCentralLocation();
    }
}
