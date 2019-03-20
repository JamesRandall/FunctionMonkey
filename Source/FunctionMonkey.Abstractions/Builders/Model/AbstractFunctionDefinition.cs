using System;
using System.Linq;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Abstractions.Extensions;

namespace FunctionMonkey.Abstractions.Builders.Model
{
    public abstract class AbstractFunctionDefinition
    {
        protected AbstractFunctionDefinition(string namePrefix, Type commandType)
        {
            if (!commandType.IsPublic)
            {
                throw new ConfigurationException($"Command of type {commandType} is not public. All command types must be public.");
            }

            Name = string.Concat(namePrefix,commandType.GetFunctionName());
            CommandType = commandType;
        }

        public string Namespace { get; set; }

        public string Name { get; set; }

        public Type CommandType { get; set; }

        public string CommandTypeName => CommandType.FullName;

        public Type CommandResultType
        {
            get
            {
                Type commandInterface = typeof(ICommand);
                Type[] interfaces = CommandType.GetInterfaces();
                Type[] minimalInterfaces = interfaces.Except(interfaces.SelectMany(i => i.GetInterfaces())).ToArray();
                Type genericCommandInterface = minimalInterfaces
                    .SingleOrDefault(x => x.IsGenericType && commandInterface.IsAssignableFrom(x));

                if (genericCommandInterface != null)
                {
                    return genericCommandInterface.GenericTypeArguments[0];
                }

                return null;
            }
        }

        public string CommandResultTypeName => CommandResultType?.FullName;

        public bool CommandHasResult => CommandResultType != null;

        public bool IsUsingValidator { get; set; }
        
        public Type CommandDeserializerType { get; set; }

        public string CommandDeseriaizerTypeName => CommandDeserializerType?.EvaluateType();
        
        public Type SerializerNamingStrategyType { get; set; }

        public string SerializerNamingStrategyTypeName => SerializerNamingStrategyType?.EvaluateType();
        
        public Type DeserializerNamingStrategyType { get; set; }

        public string DeserializerNamingStrategyTypeName => DeserializerNamingStrategyType?.EvaluateType();
      
        #region Used by the JSON compiler

        public string AssemblyName { get; set; }

        public string FunctionClassTypeName { get; set; }

        #endregion
        
        public AbstractOutputBinding OutputBinding { get; set; }
    }
}
