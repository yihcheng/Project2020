namespace P2020.ImageServiceProxy.Azure
{
    internal interface IAzureRecognizeResult
    {
        IAzureLine[] Lines { get; set; }
    }
}
