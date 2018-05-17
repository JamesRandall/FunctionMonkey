using System.Reflection;

namespace AzureFromTheTrenches.Commanding.AzureFunctions.Model
{
    internal class ClaimMapping
    {
        public string FromClaimType { get; set; }

        public PropertyInfo ToPropertyInfo { get; set; }
    }
}
