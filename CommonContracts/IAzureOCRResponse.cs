using System.Collections.Generic;

namespace CommonContracts
{
    public interface IAzureOCRResponseBoundingBoxRoot
    {
        string BoundingBox { get; set; }
    }

    public interface IAzureOCRResponseBoundingBox : IAzureOCRResponseBoundingBoxRoot
    {
        IScreenLocation GetCentralLocation();
    }

    public interface IAzureOCRResponseWord : IAzureOCRResponseBoundingBox
    {
        string Text { get; set; }
    }

    public interface IAzureOCRResponseLine: IAzureOCRResponseBoundingBox
    {
        IList<IAzureOCRResponseWord> Words { get; set; }
        string GetWords();
    }

    public interface IAzureOCRReponseRegion : IAzureOCRResponseBoundingBoxRoot
    {
        IList<IAzureOCRResponseLine> Lines { get; set; }
    }

    public interface IAzureOCRResponse
    {
        string Language { get; set; }
        decimal TextAngle { get; set; }
        string Orientation { get; set; }
        IList<IAzureOCRReponseRegion> Regions { get; set; }
    }
}
