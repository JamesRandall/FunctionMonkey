using System.Reflection;

namespace FunctionMonkey.Model
{
    internal class ClaimMapping
    {
        public string FromClaimType { get; set; }

        public PropertyInfo ToPropertyInfo { get; set; }
    }
}
