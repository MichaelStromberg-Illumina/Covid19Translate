using System.IO;

namespace Covid19Translate.Utilities
{
    public static class FileUtilities
    {
        public static FileStream GetReadStream(string path)   => new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
        public static FileStream GetCreateStream(string path) => new FileStream(path, FileMode.Create);
    }
}
