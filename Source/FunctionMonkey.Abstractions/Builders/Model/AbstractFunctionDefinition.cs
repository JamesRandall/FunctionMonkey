using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Abstractions.Extensions;
using FunctionMonkey.Commanding.Abstractions;
using FunctionMonkey.Model;

namespace FunctionMonkey.Abstractions.Builders.Model
{
    public abstract class AbstractFunctionDefinition
    {
        private readonly Type _explicitCommandResultType;
        
        protected AbstractFunctionDefinition(string namePrefix, Type commandType)
        {
            if (!commandType.IsPublic && !commandType.IsNested)
            {
                throw new ConfigurationException($"Command of type {commandType} is not public. All command types must be public.");
            }

            Name = string.Concat(namePrefix,commandType.GetFunctionName());
            CommandType = commandType;
            _explicitCommandResultType = null;
        }
        
        protected AbstractFunctionDefinition(string namePrefix, Type commandType, Type explicitCommandResultType)
        {
            if (!commandType.IsPublic && !commandType.IsNested)
            {
                throw new ConfigurationException($"Command of type {commandType} is not public. All command types must be public.");
            }

            Name = string.Concat(namePrefix,commandType.GetFunctionName());
            CommandType = commandType;
            _explicitCommandResultType = explicitCommandResultType;
        }

        public IReadOnlyCollection<ImmutableTypeConstructorParameter> ImmutableTypeConstructorParameters { get; set; }
        
        public object FunctionHandler { get; set; }

        public bool FunctionHandlerIsAsync => FunctionHandler != null &&
                                      typeof(Task).IsAssignableFrom(FunctionHandler.GetType().GetGenericArguments().Last());

        public bool IsFunctionalFunction => FunctionHandler != null;

        public string Namespace { get; set; }

        public string Name { get; set; }

        public Type CommandType { get; set; }

        public string CommandTypeName => CommandType.EvaluateType();

        public Type CommandResultType
        {
            get
            {
                if (_explicitCommandResultType != null)
                {
                    return _explicitCommandResultType;
                }
                
                if (NoCommandHandler || CommandType.GetInterfaces().Any(x => x == typeof(ICommandWithNoHandler)))
                {
                    return CommandType;
                }
                
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

        public bool CommandResultIsCollection =>
            CommandHasResult && typeof(IEnumerable).IsAssignableFrom(CommandResultType);

        // when a command result is an enumerable this returns the type of the item in the collection
        // when it is not an enumerable it just returns the type
        public string CommandResultItemTypeName => CommandResultItemType?.EvaluateType();

        public Type CommandResultItemType
        {
            get
            {
                Type itemType = CommandResultType;
                if (CommandResultType.IsGenericType && CommandResultType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                {
                    itemType = CommandResultType.GetGenericArguments().Single();
                }
                else
                {
                    Type enumerableType = CommandResultType.GetInterfaces().SingleOrDefault(x => x.IsGenericType && typeof(IEnumerable<>).IsAssignableFrom(x.GetGenericTypeDefinition()));
                    if (enumerableType != null)
                    {
                        itemType = enumerableType.GetGenericArguments().Single();
                    }
                }


                return itemType;
            }
        }

        public string CommandResultTypeName => CommandResultType.EvaluateType();

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
        
        public bool NoCommandHandler { get; set; }
    }
}
