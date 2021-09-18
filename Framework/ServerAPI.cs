using System;
using System.Text;

namespace Average.Server.Framework
{
    public static class ServerAPI
    {
        public static string RandomString()
        {
            var g = Guid.NewGuid();
            var guid = Convert.ToBase64String(g.ToByteArray());
            guid = guid.Replace("=", "");
            guid = guid.Replace("+", "");
            guid = guid.Replace("/", "");
            return guid;
        }

        public static string RemoveSpecialCharacters(string str)
        {
            var sb = new StringBuilder();

            foreach (char c in str)
            {
                if (c >= '0' && c <= '9' || c >= 'A' && c <= 'Z' || c >= 'a' && c <= 'z' || c == '.' || c == '_')
                {
                    sb.Append(c);
                }
            }

            return sb.ToString().Replace("'", "''".Replace(@"\", "\\"));
        }
    }
}
