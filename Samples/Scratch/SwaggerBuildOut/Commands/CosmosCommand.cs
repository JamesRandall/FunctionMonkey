using System;
using System.Collections.Generic;
using System.Text;
using AzureFromTheTrenches.Commanding.Abstractions;

namespace SwaggerBuildOut.Commands
{
    public class Name
    {
        public string first { get; set; }

        public string last { get; set; }
    }

    public class CosmosCommand : ICommand
    {
        public string id { get; set; }

        public Name name { get; set; }
    }
}
