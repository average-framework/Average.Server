namespace Average.Server.Framework.Utilities
{
    internal static class DirectoryUtility
    {
        public static string GetDirectoryName(string dirPath)
        {
            var results = dirPath.Split('\\');
            return results[results.Length - 1];
        }
    }
}
