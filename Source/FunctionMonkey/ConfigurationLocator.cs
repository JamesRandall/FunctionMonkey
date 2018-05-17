using System;
using System.Linq;
using System.Reflection;
using FunctionMonkey.Abstractions;

namespace FunctionMonkey
{
    public static class ConfigurationLocator
    {
        public static IFunctionAppConfiguration FindConfiguration()
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies)
            {
                IFunctionAppConfiguration configuration = FindConfiguration(assembly);
                if (configuration != null)
                {
                    return configuration;
                }
            }
            throw new ConfigurationException("Unable to find implementation of IFunctionHostBuilder");
        }

        public static IFunctionAppConfiguration FindConfiguration(Assembly assembly)
        {
            Type interfaceType = typeof(IFunctionAppConfiguration);
            Type foundType = assembly.GetTypes().FirstOrDefault(x => interfaceType.IsAssignableFrom(x) && x.IsClass);
            if (foundType != null)
            {
                return (IFunctionAppConfiguration)Activator.CreateInstance(foundType);
            }

            return null;
        }
    }
}
