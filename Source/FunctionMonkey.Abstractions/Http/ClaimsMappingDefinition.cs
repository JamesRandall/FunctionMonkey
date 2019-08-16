using System;
using System.Reflection;

namespace FunctionMonkey.Abstractions.Http
{
    public abstract class AbstractClaimsMappingDefinition
    {
        public string ClaimName { get; set; }
    }
    
    
    public class SharedClaimsMappingDefinition : AbstractClaimsMappingDefinition
    {
        public string PropertyPath { get; set; }
    }

    public class CommandPropertyClaimsMappingDefinition : AbstractClaimsMappingDefinition
    {
        public Type CommandType { get; set; }
        
        public PropertyInfo PropertyInfo { get; set; }
    }
}