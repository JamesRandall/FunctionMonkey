using System.IO;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace FunctionMonkey.Testing
{
    public static class HttpResponseExtensions
    {
        /// <summary>
        /// Simple extension method for deserializing the JSON body of a response
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="response"></param>
        /// <returns></returns>
        public static TResult GetJson<TResult>(this HttpResponse response)
        {
            using (StreamReader reader = new StreamReader(response.Body))
            {
                string json = reader.ReadToEnd();
                return JsonConvert.DeserializeObject<TResult>(json);
            }
        }
    }
}
