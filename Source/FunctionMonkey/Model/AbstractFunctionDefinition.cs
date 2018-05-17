using System;
using System.Linq;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Extensions;

namespace FunctionMonkey.Model
{
    public abstract class AbstractFunctionDefinition
    {
        protected AbstractFunctionDefinition(string namePrefix, Type commandType)
        {
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
                Type genericCommandInterface = CommandType.GetInterfaces()
                    .SingleOrDefault(x => x.IsGenericType && commandInterface.IsAssignableFrom(x));

                if (genericCommandInterface != null)
                {
                    return genericCommandInterface.GenericTypeArguments[0];
                }

                return null;
            }
        }

        public string CommandResultTypeName => CommandResultType?.FullName;

        public bool IsUsingValidator { get; set; }

        #region Used by the JSON compiler

        public string AssemblyName { get; set; }

        public string FunctionClassTypeName { get; set; }

        #endregion
    }
}
