namespace CommonContracts
{
    public interface ITemplateMatchResult
    {
        double Confidence { get; }
        int X { get; }
        int Y { get; }
        string Message { get; }
    }
}
