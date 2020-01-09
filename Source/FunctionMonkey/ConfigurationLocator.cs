using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using FunctionMonkey.Abstractions;
using FunctionMonkey.Builders;

namespace FunctionMonkey
{
    /// <summary>
    /// Class that finds the configuration class
    /// </summary>
    public static class ConfigurationLocator
    {
        public static IFunctionAppHost FindFunctionAppHost(Assembly assembly)
        {
            return Find<IFunctionAppHost>();
        }
        
        public static IFunctionAppConfiguration FindConfiguration(Assembly assembly)
        {
            return Find<IFunctionAppConfiguration>(assembly);
        }
        
        public static IFunctionCompilerMetadata FindCompilerMetadata(Assembly assembly)
        {
            if (assembly == null)
            {
                return FindCompilerMetadata();
            }

            try
            {
                foreach (Type type in assembly.GetTypes())
                {
                    foreach (PropertyInfo propertyInfo in type.GetProperties(BindingFlags.Public | BindingFlags.Static))
                    {
                        if (propertyInfo.PropertyType == typeof(IFunctionCompilerMetadata))
                        {
                            IFunctionCompilerMetadata
                                metadata = (IFunctionCompilerMetadata) propertyInfo.GetValue(null);
                            return metadata;
                        }
                    }
                }

                return null;
            }
            catch (ReflectionTypeLoadException rex)
            {
                throw RaiseTypeLoadingException(rex);
            }
        }

        private static IFunctionCompilerMetadata FindCompilerMetadata()
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
            if (TryFindCompilerMetadata(out var linkBackInfo, out var findConfiguration)) return findConfiguration;

            // If we ge there then we found (2) but not an implementation of IFunctionAppConfiguration. That being the case
            // we call that method. This will force the assembly to load, we rescan, and we should now find our implementation.
            //
            // This is a bit long-winded but I'm trying to avoid loading referenced assemblies myself in an already pretty
            // complex runtime environment that is likely to already to have various resolution hooks.
            if (linkBackInfo != null)
            {
                linkBackInfo.Invoke(null, null);
                if (TryFindCompilerMetadata(out var dummyLinkBackInfo, out var secondAttemptFindConfiguration)) return secondAttemptFindConfiguration;
            }

            return null;
        }
        
        private static bool TryFindCompilerMetadata(out MethodInfo linkBackInfo, out IFunctionCompilerMetadata findConfiguration)
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            linkBackInfo = null;
            findConfiguration = null;

            foreach (Assembly assembly in assemblies)
            {
                IFunctionCompilerMetadata configuration = FindCompilerMetadata(assembly);
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
        
        private static TType Find<TType>() where TType : class
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
            if (Scan<TType>(out var linkBackInfo, out var findConfiguration)) return findConfiguration;

            // If we ge there then we found (2) but not an implementation of IFunctionAppConfiguration. That being the case
            // we call that method. This will force the assembly to load, we rescan, and we should now find our implementation.
            //
            // This is a bit long-winded but I'm trying to avoid loading referenced assemblies myself in an already pretty
            // complex runtime environment that is likely to already to have various resolution hooks.
            if (linkBackInfo != null)
            {
                linkBackInfo.Invoke(null, null);
                if (Scan<TType>(out var dummyLinkBackInfo, out var secondAttemptFindConfiguration)) return secondAttemptFindConfiguration;
            }

            return default(TType);
            //throw new ConfigurationException("Unable to find implementation of IFunctionAppConfiguration");
        }

        private static bool Scan<TType>(out MethodInfo linkBackInfo, out TType findConfiguration) where TType : class
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            linkBackInfo = null;
            findConfiguration = null;

            foreach (Assembly assembly in assemblies)
            {
                TType configuration = Find<TType>(assembly);
                if (configuration != null)
                {
                    {
                        findConfiguration = configuration;
                        return true;
                    }
                }

                string assemblyName = assembly.GetName().Name;
                if (assemblyName.EndsWith(".Functions"))
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

        private static TType Find<TType>(Assembly assembly) where TType : class
        {
            if (assembly == null)
            {
                return Find<TType>();
            }

            try
            {
                Type interfaceType = typeof(TType);
                Type foundType = assembly.GetTypes()
                    .FirstOrDefault(x => interfaceType.IsAssignableFrom(x) && x.IsClass);
                if (foundType != null)
                {
                    return (TType) Activator.CreateInstance(foundType);
                }

                return null;
            }
            catch (ReflectionTypeLoadException rex)
            {
                throw RaiseTypeLoadingException(rex);
            }
        }

        private static TypeLoadingException RaiseTypeLoadingException(ReflectionTypeLoadException rex)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"ReflectionTypeLoadException: {rex.Message}");
            if (rex.Types != null)
            {
                sb.AppendLine("Unable to load types:");
                foreach (Type loaderType in rex.Types)
                {
                    sb.AppendLine(loaderType != null
                        ? $"    {loaderType.FullName}"
                        : "    null type in ReflectionTypeLoadException");
                }
            }

            if (rex.LoaderExceptions != null)
            {
                sb.AppendLine("With errors:");
                foreach (var loaderException in rex.LoaderExceptions)
                {
                    sb.AppendLine(loaderException != null
                        ? $"    {loaderException.GetType().Name}: {loaderException.Message}"
                        : "    null LoadedException in ReflectionTypeLoadException");
                }
            }

            return new TypeLoadingException(sb.ToString());
        }
    }
}
