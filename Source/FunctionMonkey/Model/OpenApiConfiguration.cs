using System.Collections.Generic;

namespace AzureFromTheTrenches.Commanding.AzureFunctions.Model
{
    public class OpenApiConfiguration
    {
        public string Version { get; set; }

        public string Title { get; set; }

        public IReadOnlyCollection<string> Servers { get; set; }

        public bool IsOpenApiOutputEnabled => !string.IsNullOrWhiteSpace(Version) && !string.IsNullOrWhiteSpace(Title);

        public string UserInterfaceRoute { get; set; }
    }
}
