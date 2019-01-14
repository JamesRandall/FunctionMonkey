using FunctionMonkey.Abstractions.Builders;
using System.Collections.Generic;

namespace FunctionMonkey.Model
{
    public class OpenApiConfiguration
    {
        public string Version { get; set; }

        public string Title { get; set; }

        public IReadOnlyCollection<string> Servers { get; set; }

        public ApiSpecVersion ApiSpecVersion { get; set; }

        public ApiOutputFormat ApiOutputFormat { get; set; }

        public bool IsOpenApiOutputEnabled => !string.IsNullOrWhiteSpace(Version) && !string.IsNullOrWhiteSpace(Title);

        public bool IsValid
        {
            get
            {
                int requiredSettingCount = 0;
                if (!string.IsNullOrWhiteSpace(Version)) requiredSettingCount++;
                if (!string.IsNullOrWhiteSpace(Title)) requiredSettingCount++;

                return requiredSettingCount == 0 || requiredSettingCount == 2;
            }
        }        

        public string UserInterfaceRoute { get; set; }
    }
}
