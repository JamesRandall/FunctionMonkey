using System;
using System.Collections.Generic;
using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace FunctionMonkey.FluentValidation
{
    /// <summary>
    /// The extensions in this class are brought in from the FluentValidation source: https://github.com/JeremySkinner/FluentValidation
    /// They are copyright the original author as per this (Apache 2.0) license:
    /// https://github.com/JeremySkinner/FluentValidation/blob/5d86531ba7dcfade6055c00a5dc45ed32660148d/License.txt
    /// FluentValidation only includes them as part of its broader ASP.Net Core package that we don't want to reference here
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public static class IServiceCollectionExtensions
    {
        /// <summary>
		/// Adds all validators in specified assemblies
		/// </summary>
		/// <param name="services">The collection of services</param>
		/// <param name="assembly">The assemblies to scan</param>
		/// <param name="lifetime">The lifetime of the validators. The default is transient</param>
		/// <returns></returns>
		public static IServiceCollection AddValidatorsFromAssemblies(this IServiceCollection services, IEnumerable<Assembly> assemblies, ServiceLifetime lifetime = ServiceLifetime.Transient)
        {
            foreach (var assembly in assemblies)
                services.AddValidatorsFromAssembly(assembly, lifetime);

            return services;
        }

        /// <summary>
        /// Adds all validators in specified assembly
        /// </summary>
        /// <param name="services">The collection of services</param>
        /// <param name="assembly">The assembly to scan</param>
        /// <param name="lifetime">The lifetime of the validators. The default is transient</param>
        /// <returns></returns>
        public static IServiceCollection AddValidatorsFromAssembly(this IServiceCollection services, Assembly assembly, ServiceLifetime lifetime = ServiceLifetime.Transient)
        {
            AssemblyScanner
                .FindValidatorsInAssembly(assembly)
                .ForEach(scanResult => services.AddScanResult(scanResult, lifetime));

            return services;
        }

        /// <summary>
        /// Adds all validators in the assembly of the specified type
        /// </summary>
        /// <param name="services">The collection of services</param>
        /// <param name="type">The type whose assembly to scan</param>
        /// <param name="lifetime">The lifetime of the validators. The default is transient</param>
        /// <returns></returns>
        public static IServiceCollection AddValidatorsFromAssemblyContaining(this IServiceCollection services, Type type, ServiceLifetime lifetime = ServiceLifetime.Transient)
            => services.AddValidatorsFromAssembly(type.Assembly, lifetime);

        /// <summary>
        /// Adds all validators in the assembly of the type specified by the generic parameter
        /// </summary>
        /// <param name="services">The collection of services</param>
        /// <param name="lifetime">The lifetime of the validators. The default is transient</param>
        /// <returns></returns>
        public static IServiceCollection AddValidatorsFromAssemblyContaining<T>(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Transient)
            => services.AddValidatorsFromAssembly(typeof(T).Assembly, lifetime);

        /// <summary>
        /// Helper method to register a validator from an AssemblyScanner result
        /// </summary>
        /// <param name="services">The collection of services</param>
        /// <param name="scanResult">The scan result</param>
        /// <param name="lifetime">The lifetime of the validators. The default is transient</param>
        /// <returns></returns>
        private static IServiceCollection AddScanResult(this IServiceCollection services, AssemblyScanner.AssemblyScanResult scanResult, ServiceLifetime lifetime)
        {
            //Register as interface
            services.Add(
                new ServiceDescriptor(
                            serviceType: scanResult.InterfaceType,
                            implementationType: scanResult.ValidatorType,
                            lifetime: lifetime));

            //Register as self
            services.Add(
                new ServiceDescriptor(
                            serviceType: scanResult.ValidatorType,
                            implementationType: scanResult.ValidatorType,
                            lifetime: lifetime));

            return services;
        }
    }
}
