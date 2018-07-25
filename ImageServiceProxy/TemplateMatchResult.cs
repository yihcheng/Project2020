using CommonContracts;

namespace ImageServiceProxy
{
    public class TemplateMatchResult : ITemplateMatchResult
    {
        public TemplateMatchResult(double confidence, int x, int y, string message)
        {
            Confidence = confidence;
            X = x;
            Y = y;
            Message = message;
        }

        public double Confidence { get; }
        public int X { get; }
        public int Y { get; }
        public string Message { get; }
    }
}
