using Average.Server.Framework.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Average.Server.Framework.Extensions
{
    internal static class ObjectExtensions
    {
        internal static string ToJson(this object source, Formatting formatting = Formatting.None)
        {
            return JsonConvert.SerializeObject(source, formatting);
        }

        internal static T Convert<T>(this object source)
        {
            if (source.GetType() == typeof(JArray))
            {
                return ((JArray)source).ToObject<T>();
            }

            try
            {
                return JsonConvert.DeserializeObject<T>(source.ToString());
            }
            catch(Exception ex)
            {
                try
                {
                    Logger.Error("r1");

                    return (T)System.Convert.ChangeType(source, typeof(T));
                }
                catch
                {
                    Logger.Error("r2");
                }
            }

            return default;
        }

        internal static T ToType<T>(this object source) => JsonConvert.DeserializeAnonymousType(JsonConvert.SerializeObject(source), (T)Activator.CreateInstance(typeof(T)));
    }
}
