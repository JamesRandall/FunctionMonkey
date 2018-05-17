using System;
using System.Reflection;

namespace AzureFromTheTrenches.Commanding.AzureFunctions.Model
{
    public class CommandClaimMappingDefinition
    {
        public string ClaimType { get; set; }

        public Type CommandType { get; set; }

        public PropertyInfo PropertyInfo { get; set; }
    }
}
