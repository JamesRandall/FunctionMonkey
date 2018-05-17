using System;

namespace AzureFromTheTrenches.Commanding.AzureFunctions
{
    public class ClaimMappingCompilationException : Exception
    {
        public Type CommandType { get; }
        public string ClaimType { get; }
        public string Name { get; }
        public Type PropertyType { get; }

        internal ClaimMappingCompilationException(Type commandType, string fromClaimType, string name, Type propertyType)
            : base("Properties used in claim mapping must be of type string or support a single parameter static parse method e.g. int.Parse(myClaim)")
        {
            CommandType = commandType;
            ClaimType = fromClaimType;
            Name = name;
            PropertyType = propertyType;
        }
    }
}
