using System.IO;
using static CitizenFX.Core.Native.API;

namespace Average.Server.Framework.Utilities
{
    internal static class FileUtility
    {
        internal static string GetRootDir()
        {
            return GetResourcePath(GetCurrentResourceName());
        }

        internal static bool Exists(string path)
        {
            return File.Exists(path);
        }

        internal static bool ExistsFromRootDir(string path)
        {
            return File.Exists(string.Join(@"\", GetRootDir(), path));
        }

        internal static string ReadFile(string path)
        {
            return File.ReadAllText(path);
        }

        internal static string ReadFileFromRootDir(string path)
        {
            return File.ReadAllText(string.Join(@"\", GetRootDir(), path));
        }

        internal static void WriteFile(string path, string content)
        {
            File.WriteAllText(path, content);
        }

        internal static void WriteFileFromRootDir(string path, string content)
        {
            File.WriteAllText(string.Join(@"\", GetRootDir(), path), content);
        }
    }
}
