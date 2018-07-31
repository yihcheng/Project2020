namespace ImageServiceProxy.Azure
{
    internal interface IAzureRecognizeTextResponse
    {
        string Status { get; set; }
        IAzureRecognizeResult RecognitionResult { get; set; }
    }
}
