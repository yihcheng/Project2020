using System.IO;
using P2020.Abstraction;

namespace P2020.ImageServiceProxy
{
    public class OpenCVService : IOpenCVSUtils
    {
        public OpenCVService()
        {
        }

        /// <summary>
        /// This is OpenCV MatchTemplate CCoeffNormed mode
        /// </summary>
        /// <param name="search"></param>
        /// <param name="template"></param>
        /// <returns>(double, int, int, int, int) = confidence and top-left corner X,Y, width, and height</returns>
        public (double Confidence, int X, int Y, int Width, int Height)? TemplateMatch(byte[] search, byte[] template)
        {
            if (search == null
                || template == null
                || search.Length == 0
                || template.Length == 0)
            {
                return null;
            }

            double maxval;
            Point maxloc = default;
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

            if (maxloc == default(Point))
            {
                return null;
            }

            return (maxval, maxloc.X, maxloc.Y, searchImageWidth, searchImageHeight);
        }

        public void DrawRedRectangle(string imageFile, int X, int Y, int width, int height)
        {
            if (string.IsNullOrEmpty(imageFile)
                || !File.Exists(imageFile)
                || X < 0
                || Y < 0
                || width < 0
                || height < 0)
            {
                return;
            }

            Mat sourceMat = new Mat(imageFile, ImreadModes.AnyDepth | ImreadModes.AnyColor);
            Rect rect = new Rect(X, Y, width, height);
            Cv2.Rectangle(sourceMat, rect, Scalar.Red, 2);
            sourceMat.SaveImage(imageFile);
        }

        public void PutText(string imageFile, int X, int Y, string message)
        {
            if (string.IsNullOrEmpty(imageFile)
             || !File.Exists(imageFile)
             || X < 0
             || Y < 0
             || string.IsNullOrEmpty(message))
            {
                return;
            }

            Mat sourceMat = new Mat(imageFile, ImreadModes.AnyDepth | ImreadModes.AnyColor);
            sourceMat.PutText(message, new Point(X, Y), HersheyFonts.HersheyComplexSmall, 1, Scalar.Red, 1);
            sourceMat.SaveImage(imageFile);
        }
    }
}
