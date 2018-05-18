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
            // We scan the assemblies looking for:
            //
            // 1. An implementation of IFunctionAppConfiguration
            //
            // however the assembly containing it may not have been loaded (it its empty other than the builder) so next
            // we look for:
            //
            // 2. A class called ReferenceLinkBack that has the ReferenceLinkBackAttribute on it and a single static
            // method called ForceLinkBack
            if (Scan(out var linkBackInfo, out var findConfiguration)) return findConfiguration;

            // If we ge there then we found (2) but not an implementation of IFunctionAppConfiguration. That being the case
            // we call that method. This will force the assembly to load, we rescane, and we should now find our implementation.
            //
            // This is a bit long-winded but I'm trying to avoid loading referenced assemblies myself in an already pretty
            // complex runtime environment that is likely to already to have various resolution hooks.
            if (linkBackInfo != null)
            {
                linkBackInfo.Invoke(null, null);
                if (Scan(out var dummyLinkBackInfo, out var secondAttemptFindConfiguration)) return secondAttemptFindConfiguration;
            }

            throw new ConfigurationException("Unable to find implementation of IFunctionAppConfiguration");
        }

        private static bool Scan(out MethodInfo linkBackInfo, out IFunctionAppConfiguration findConfiguration)
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            linkBackInfo = null;
            findConfiguration = null;

            foreach (Assembly assembly in assemblies)
            {
                IFunctionAppConfiguration configuration = FindConfiguration(assembly);
                if (configuration != null)
                {
                    {
                        findConfiguration = configuration;
                        return true;
                    }
                }

                string assemblyName = assembly.GetName().Name;
                if (assemblyName.EndsWith(".Functions.dll"))
                {
                    Type[] candidateReferenceLinkBackTypes = assembly.GetTypes()
                        .Where(x => x.Name == "ReferenceLinkBack" && x.IsAbstract && x.IsSealed).ToArray();
                    if (candidateReferenceLinkBackTypes.Length == 1)
                    {
                        if (candidateReferenceLinkBackTypes[0].GetCustomAttributes<ReferenceLinkBackAttribute>().Any())
                        {
                            linkBackInfo = candidateReferenceLinkBackTypes[0].GetMethod("ForceLinkBack");
                        }
                    }
                }
            }

            return false;
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
