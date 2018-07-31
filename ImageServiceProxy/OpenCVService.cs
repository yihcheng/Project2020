using CommonContracts;
using OpenCvSharp;

namespace ImageServiceProxy
{
    internal class OpenCVService : IOpenCVService
    {
        public OpenCVService()
        {

        }

        // This is CCoeffNormed mode
        public (double, int, int)? TemplateMatch(byte[] search, byte[] template)
        {
            if (search == null
                || template == null
                || search.Length == 0
                || template.Length == 0)
            {
                return null;
            }

            double maxval;
            Point maxloc = default(Point);
            int searchImageWidth = 0;
            int searchImageHeight = 0;

            using (Mat refMat = Cv2.ImDecode(search, ImreadModes.Color))
            using (Mat tplMat = Cv2.ImDecode(template, ImreadModes.Color))
            using (Mat res = new Mat())
            {
                Mat gref = refMat.CvtColor(ColorConversionCodes.BGR2GRAY);
                Mat gtpl = tplMat.CvtColor(ColorConversionCodes.BGR2GRAY);

                Cv2.MatchTemplate(gref, gtpl, res, TemplateMatchModes.CCoeffNormed);
                Cv2.MinMaxLoc(res, out double minval, out maxval, out Point minloc, out maxloc);
                searchImageWidth = gref.Size().Width;
                searchImageHeight = gref.Size().Height;

                gref.Dispose();
                gtpl.Dispose();
            }

            int X = maxloc.X + (searchImageWidth / 2);
            int Y = maxloc.Y + (searchImageHeight / 2);

            if (maxloc == default(Point))
            {
                return null;
            }

            return (maxval, X, Y);
        }
    }
}
