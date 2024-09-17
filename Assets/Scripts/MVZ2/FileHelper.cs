using System.IO;

namespace MVZ2
{
    public static class FileHelper
    {
        public static void ValidateDirectory(string filePath)
        {
            var dir = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
        }
    }
}
