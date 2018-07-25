using System.IO;

namespace Engine
{
    internal static class FileUtility
    {
        public static void CreateParentFolder(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return;
            }

            string dir = Path.GetDirectoryName(filePath);

            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
        }
    }
}
