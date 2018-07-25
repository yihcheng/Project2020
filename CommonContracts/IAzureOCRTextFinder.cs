using System.Collections.Generic;

namespace CommonContracts
{
    public interface IAzureOCRTextFinder
    {
        bool TrySearchText(string textToSearch, ScreenSearchArea searchArea, out IScreenLocation location);
        bool TryGetAllLinesInRegion(int regionId, out IList<IAzureOCRResponseLine> value);
    }

    public interface IAzureRecognizeTextFinder
    {
        bool TrySearchText(string textToSearch, ScreenSearchArea searchArea, out IScreenLocation location);
    }
}
