namespace CommonContracts
{
    // TODO: should have a better name
    public interface IWord
    {
        int[] BoundingBox { get; set; }
        string Text { get; set; }
        IScreenLocation GetCentralLocation();
    }
}
