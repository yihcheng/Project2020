using System.IO;

namespace P2020.Engine
{
    internal static class FileUtility
    {
        public static bool EnsureParentFolder(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return false;
            }

            try
            {
                string dir = Path.GetDirectoryName(filePath);

                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}
