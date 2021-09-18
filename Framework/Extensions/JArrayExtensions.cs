using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace Average.Server.Framework.Extensions
{
    internal static class JArrayExtensions
    {
        internal static List<T> ToList<T>(this JArray array)
        {
            return array.Cast<T>().ToList();
        }
    }
}
