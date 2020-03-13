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
    public class BridgedFunction
    {
        public BridgedFunction(object handler)
        {
            Handler = handler;
        }
        
        public object Handler { get; }
        
        public bool IsAsync => typeof(Task).IsAssignableFrom(Handler.GetType().GetGenericArguments().Last());
    }
    
    public abstract class AbstractFunctionDefinition
    {
        private readonly Type _explicitCommandResultType;
        
        protected AbstractFunctionDefinition(string namePrefix, Type commandType)
        {
            /*if (!commandType.IsPublic && !commandType.IsNested)
            {
                throw new ConfigurationException($"Command of type {commandType} is not public. All command types must be public.");
            }*/

            Name = string.Concat(namePrefix,commandType.GetFunctionName());
            CommandType = commandType;
            _explicitCommandResultType = null;
        }
        
        protected AbstractFunctionDefinition(string namePrefix, Type commandType, Type explicitCommandResultType)
        {
            /*if (!commandType.IsPublic && !commandType.IsNested)
            {
                throw new ConfigurationException($"Command of type {commandType} is not public. All command types must be public.");
            }*/

            Name = string.Concat(namePrefix,commandType.GetFunctionName());
            CommandType = commandType;
            _explicitCommandResultType = explicitCommandResultType;
            CommandResultType = explicitCommandResultType;
        }

        public IReadOnlyCollection<ImmutableTypeConstructorParameter> ImmutableTypeConstructorParameters { get; set; }

        public Type ExplicitCommandResultType => _explicitCommandResultType;

        public string Namespace { get; set; }

        public string Name { get; set; }

        public Type CommandType { get; set; }

        public bool CommandTypeIsUnit => CommandType.FullName == "Microsoft.FSharp.Core.Unit";

        public string CommandTypeName => CommandType.EvaluateType();

        public Type CommandResultType { get; set; }

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

        public bool CommandResultTypeIsUnit => CommandResultType?.FullName == "Microsoft.FSharp.Core.Unit";

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
        
        // F# support
        
        // If set to true then the command class must expose a constructor with each property a parameter.
        public bool UsesImmutableTypes { get; set; }
        
        public object FunctionHandler { get; set; }

        public bool FunctionHandlerIsAsync => FunctionHandler != null &&
                                              typeof(Task).IsAssignableFrom(FunctionHandler.GetType().GetGenericArguments().Last());

        public bool IsFunctionalFunction => FunctionHandler != null;
        
        public BridgedFunction ValidatorFunction { get; set; }
        
        public BridgedFunction DeserializeFunction { get; set; }
        
        public BridgedFunction SerializeFunction { get; set; }
        
        public BridgedFunction IsValidFunction { get; set; }
        
        // we have to use a string name comparison here as we don't want to bring in the  FSharp assembly
        public bool ResultIsFSharpUnit => CommandResultType?.FullName == "Microsoft.FSharp.Core.Unit";
        
        public Type CommandTransformerType { get; set; }

        public string CommandTransformerTypeName => CommandTransformerType.EvaluateType();

        public bool HasCommandTransformer => CommandTransformerType != null;
        public bool HasCommandHandlerOverride { get; set; }
    }
}
