﻿namespace Average.Server.Utilities
{
    public static class Directory
    {
        public static string GetDirectoryName(string dirPath)
        {
            var results = dirPath.Split('\\');
            return results[results.Length - 1];
        }
    }
}
