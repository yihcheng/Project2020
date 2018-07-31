namespace ImageServiceProxy.Azure
{
    internal interface IAzureRecognizeResult
    {
        IAzureLine[] Lines { get; set; }
    }
}
