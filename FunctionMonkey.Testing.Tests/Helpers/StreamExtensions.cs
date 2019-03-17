using System.IO;
using Newtonsoft.Json;

namespace FunctionMonkey.Testing.Tests.Helpers
{
    internal static class StreamExtensions
    {
        public static TResult DeserializeObject<TResult>(this Stream stream)
        {
            using (StreamReader reader = new StreamReader(stream))
            {
                string json = reader.ReadToEnd();
                return JsonConvert.DeserializeObject<TResult>(json);
            }
        }

        public static string GetString(this Stream stream)
        {
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
