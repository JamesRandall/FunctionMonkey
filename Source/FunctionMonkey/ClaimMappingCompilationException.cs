using System;

namespace FunctionMonkey
{
    /// <summary>
    /// Exception thrown when their are issues compiling claims mappers
    /// </summary>
    public class ClaimMappingCompilationException : Exception
    {
        /// <summary>
        /// The command type
        /// </summary>
        public Type CommandType { get; }
        /// <summary>
        /// The claim type
        /// </summary>
        public string ClaimType { get; }
        /// <summary>
        /// The name
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// The property type
        /// </summary>
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
