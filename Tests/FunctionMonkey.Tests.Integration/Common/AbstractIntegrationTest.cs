using System.IO;
using Flurl;
using Flurl.Http;
using Newtonsoft.Json;

namespace FunctionMonkey.Tests.Integration.Common
{
    public class AbstractIntegrationTest
    {
        public static readonly Settings Settings = null;

        static AbstractIntegrationTest()
        {
            string settingsJson = File.ReadAllText("./appsettings.json");
            Settings = JsonConvert.DeserializeObject<Settings>(settingsJson);

            Settings.Host
                .AppendPathSegment("setup")
                .PutAsync(null)
                .Wait();
        }
    }
}
