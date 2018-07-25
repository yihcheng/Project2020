namespace CommonContracts
{
    public interface IAzureRecognizeTextResponse
    {
        string Status { get; set; }
        IRecognizeResult RecognitionResult { get; set; }
    }
}
