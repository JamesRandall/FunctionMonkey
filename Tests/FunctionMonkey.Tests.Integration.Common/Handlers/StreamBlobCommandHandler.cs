using System;
using System.IO;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Common.Commands;
using FunctionMonkey.Tests.Integration.Common.Extensions;
using Newtonsoft.Json.Linq;

namespace FunctionMonkey.Tests.Integration.Common.Handlers
{
    class StreamBlobCommandHandler : ICommandHandler<StreamBlobCommand>
    {
        public async Task ExecuteAsync(StreamBlobCommand command)
        {
            string filenameMarkerString = Path.GetFileNameWithoutExtension(command.Name);

            using (StreamReader reader = new StreamReader(command.Stream))
            {
                string json = reader.ReadToEnd();
                JObject jObject = JObject.Parse(json);
                string markerIdProperty = jObject["MarkerId"].Value<string>();
                Guid commandMarkerId = Guid.Parse(markerIdProperty);

                Guid filenameMarker = string.IsNullOrWhiteSpace(filenameMarkerString) ? Guid.Empty : Guid.Parse(filenameMarkerString);

                if (filenameMarker == commandMarkerId)
                {
                    await filenameMarker.RecordMarker();
                }
            }                
        }
    }
}
