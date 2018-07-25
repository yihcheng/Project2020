namespace CommonContracts
{
    // TODO: should have a better name
    public interface ILine
    {
        int[] BoundingBox { get; set; }
        string Text { get; set; }
        IWord[] Words { get; set; }
        IScreenLocation GetCentralLocation();
    }
}
