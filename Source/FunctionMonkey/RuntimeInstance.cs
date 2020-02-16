using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Abstractions;
using FunctionMonkey.Abstractions.Builders;
using FunctionMonkey.Abstractions.Builders.Model;
using FunctionMonkey.Abstractions.Http;
using FunctionMonkey.Builders;
using FunctionMonkey.Commanding.Abstractions.Validation;
using FunctionMonkey.Compiler.Core;
using FunctionMonkey.Infrastructure;
using FunctionMonkey.Model;
using FunctionMonkey.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;

namespace FunctionMonkey
{

    public abstract class AbstractPluginFunctions
    {
        
    }

    public class RuntimeInstance
    {
        public IServiceProvider ServiceProvider => FunctionServiceProvider.Value ?? BuiltServiceProvider.Value;

        public AsyncLocal<IServiceProvider> FunctionServiceProvider { get; } = new AsyncLocal<IServiceProvider>(null);

        public AsyncLocal<ILogger> FunctionProvidedLogger { get;  }= new AsyncLocal<ILogger>(null);

        public Dictionary<string, PluginFunctions> PluginFunctions { get; } = new Dictionary<string, PluginFunctions>();
        
        //public IFunctionHostBuilder Builder { get; }
        public IReadOnlyCollection<AbstractFunctionDefinition> FunctionDefinitions { get; }

        public IFunctionHostBuilder Builder { get; private set; }

        public Lazy<IServiceProvider> BuiltServiceProvider { get; }

        private IServiceCollection ServiceCollection { get; }

        public RuntimeInstance(Assembly functionAppConfigurationAssembly,
            Action<IServiceCollection, ICommandRegistry> beforeServiceProviderBuild,
            IServiceCollection serviceCollection)
        {
            if (serviceCollection == null)
            {
                System.Console.WriteLine("No service collection supplied by runtime");
            }
            ServiceCollection = serviceCollection ?? new ServiceCollection();
            BuiltServiceProvider = new Lazy<IServiceProvider>(() => ServiceCollection.BuildServiceProvider());

            FunctionAppHostBuilder appHostBuilder = null;
            IFunctionAppConfiguration configuration = null;
            IFunctionAppHost appHost = ConfigurationLocator.FindFunctionAppHost(functionAppConfigurationAssembly);
            if (appHost != null)
            {
                appHostBuilder = new FunctionAppHostBuilder();
                appHost.Build(appHostBuilder);
                if (appHostBuilder.FunctionAppConfiguration != null)
                {
                    configuration = (IFunctionAppConfiguration)Activator.CreateInstance(appHostBuilder.FunctionAppConfiguration);
                }
            }

            if (configuration == null)
            {
                configuration = ConfigurationLocator.FindConfiguration(functionAppConfigurationAssembly);
            }
            
            CommandingDependencyResolverAdapter adapter = new CommandingDependencyResolverAdapter(
                (fromType, toInstance) => ServiceCollection.AddSingleton(fromType, toInstance),
                (fromType, toType) => ServiceCollection.AddTransient(fromType, toType),
                resolveType => ServiceProvider.GetService(resolveType)
            );

            ICommandRegistry commandRegistry;
            // ReSharper disable once SuspiciousTypeConversion.Global - externally provided
            if (configuration != null && configuration is ICommandingConfigurator commandingConfigurator)
            {
                commandRegistry = commandingConfigurator.AddCommanding(adapter);
            }
            else
            {
                CommandingRuntime commandingRuntime = new CommandingRuntime();
                commandRegistry = commandingRuntime.AddCommanding(adapter);
            }

            // Register internal implementations
            RegisterInternalImplementations();
            
            FunctionHostBuilder builder = null;
            IFunctionCompilerMetadata functionCompilerMetadata = null;
            CompileTargetEnum compileTarget;
            if (configuration != null)
            {
                // Invoke the builder process
                builder = CreateBuilderFromConfiguration(commandRegistry, configuration);
                if (appHostBuilder != null)
                {
                    builder.Options = appHostBuilder.Options;
                }
                FunctionBuilder functionBuilder = (FunctionBuilder)builder.FunctionBuilder;
                FunctionDefinitions = builder.FunctionDefinitions;
                compileTarget = builder.Options.HttpTarget;
                SetupAuthorization(builder, functionBuilder);
            }
            else
            {
                functionCompilerMetadata = LocateFunctionCompilerMetadata(functionAppConfigurationAssembly);
                FunctionDefinitions = functionCompilerMetadata.FunctionDefinitions;
                compileTarget = functionCompilerMetadata.CompilerOptions.HttpTarget;
            }
            
            PostBuildPatcher.EnsureFunctionsHaveUniqueNames(FunctionDefinitions);

            RegisterCoreDependencies(builder?.MediatorType ?? typeof(DefaultMediatorDecorator), FunctionDefinitions, compileTarget);

            RegisterTimerCommandFactories(FunctionDefinitions);

            RegisterHttpDependencies(FunctionDefinitions);

            RegisterCosmosDependencies(FunctionDefinitions);

            RegisterOutputBindingDependencies(FunctionDefinitions);

            
            CreatePluginFunctions(functionCompilerMetadata?.ClaimsMappings, FunctionDefinitions);

            RegisterLoggerIfRequired();
        }

        private void RegisterOutputBindingDependencies(IReadOnlyCollection<AbstractFunctionDefinition> functionDefinitions)
        {
            HashSet<Type> types = new HashSet<Type>();
            foreach (AbstractFunctionDefinition abstractFunctionDefinition in functionDefinitions)
            {
                if (abstractFunctionDefinition.OutputBinding?.OutputBindingConverterType != null)
                {
                    types.Add(abstractFunctionDefinition.OutputBinding.OutputBindingConverterType);
                }
            }

            foreach (Type type in types)
            {
                ServiceCollection.AddTransient(type, type);
            }
        }

        private void RegisterLoggerIfRequired()
        {
            // Ensure we have an uncategorised ILogger registered, we may not in ASP.Net Core
            if (ServiceCollection.All(x => x.ServiceType != typeof(ILogger)))
            {
                if (ServiceCollection.Any(x => x.ServiceType == typeof(ILoggerFactory)))
                {
                    ServiceCollection.AddTransient(typeof(ILogger), sp =>
                    {
                        ILoggerFactory loggerFactory = sp.GetService<ILoggerFactory>();
                        return loggerFactory.CreateLogger("common");
                    });
                }
            }
        }

        private ISerializer CreateSerializer(AbstractFunctionDefinition functionDefinition)
        {
            ISerializer serializer;
            if (functionDefinition.SerializerNamingStrategyType != null)
            {
                var deserializerNamingStrategy = (NamingStrategy)
                    Activator.CreateInstance(functionDefinition.DeserializerNamingStrategyType);
                var serializerNamingStrategy = (NamingStrategy)
                    Activator.CreateInstance(functionDefinition.SerializerNamingStrategyType);
                serializer =
                    new NamingStrategyJsonSerializer(deserializerNamingStrategy, serializerNamingStrategy);
            }
            else
            {
                serializer = (FunctionMonkey.Abstractions.ISerializer)
                    ServiceProvider.GetService(functionDefinition.CommandDeserializerType);
            }

            return serializer;
        }

        private void CreatePluginFunctions(
            IReadOnlyCollection<AbstractClaimsMappingDefinition> claimsMappings,
            IReadOnlyCollection<AbstractFunctionDefinition> functionDefinitions)
        {
            foreach (AbstractFunctionDefinition functionDefinition in functionDefinitions)
            {
                PluginFunctions pluginFunctions = new PluginFunctions();
                
                pluginFunctions.Handler = functionDefinition.FunctionHandler;

                if (functionDefinition.OutputBinding != null)
                {
                    if (functionDefinition.OutputBinding.OutputBindingConverterType != null)
                    {
                        pluginFunctions.OutputBindingConverter = (originatingCommand, input) =>
                        {
                            IOutputBindingConverter converter =
                                (IOutputBindingConverter) ServiceProvider.GetService(functionDefinition.OutputBinding
                                    .OutputBindingConverterType);
                            return converter.Convert(originatingCommand, input);
                        };
                    }
                    else if (functionDefinition.OutputBinding.OutputBindingConverterFunction != null)
                    {
                        pluginFunctions.OutputBindingConverter = (originatingCommand, input) =>
                            ((Func<object, object, object>) functionDefinition.OutputBinding.OutputBindingConverterFunction
                                .Handler)(originatingCommand, input);
                    }
                }

                if (functionDefinition.DeserializeFunction != null)
                {
                    pluginFunctions.Deserialize = (body, enforceSecurityProperties) =>
                        ((Func<string, bool, object>) functionDefinition.SerializeFunction.Handler)(body, enforceSecurityProperties);
                }
                else
                {
                    pluginFunctions.Deserialize = (body, enforceSecurityProperties) =>
                        CreateSerializer(functionDefinition).Deserialize(functionDefinition.CommandType, body,
                            enforceSecurityProperties);
                }

                if (functionDefinition.SerializeFunction != null)
                {
                    pluginFunctions.Serialize = (obj, enforceSecurityProperties) =>
                        ((Func<object, bool, string>) functionDefinition.SerializeFunction.Handler)(obj,
                            enforceSecurityProperties);
                }
                else
                {
                    pluginFunctions.Serialize = (content, enforceSecurityProperties) =>
                        CreateSerializer(functionDefinition).Serialize(content, enforceSecurityProperties);
                }

                if (functionDefinition.ValidatorFunction != null)
                {
                    pluginFunctions.Validate = obj =>
                        ((Func<object, object>) functionDefinition.ValidatorFunction.Handler)(obj);
                }
                else
                {
                    pluginFunctions.Validate = command =>
                    {
                        var validator = (FunctionMonkey.Abstractions.Validation.IValidator)
                            ServiceProvider.GetService(
                                typeof(FunctionMonkey.Abstractions.Validation.IValidator));
                        var validationResult = validator.Validate(command);
                        return validationResult;
                    };
                }
                
                if (functionDefinition.IsValidFunction != null)
                {
                    pluginFunctions.IsValid = (Func<object,bool>)functionDefinition.IsValidFunction.Handler;
                }
                else
                {
                    pluginFunctions.IsValid = validationResult =>
                    {
                        ValidationResult castValidationResult = (ValidationResult) validationResult;
                        return castValidationResult.IsValid;
                    };
                }
                
                if (functionDefinition is HttpFunctionDefinition httpFunctionDefinition)
                {
                    if (httpFunctionDefinition.TokenValidatorFunction != null)
                    {
                        if (httpFunctionDefinition.TokenValidatorFunction.IsAsync)
                        {
                            pluginFunctions.ValidateToken =
                                (Func<string, Task<ClaimsPrincipal>>) httpFunctionDefinition.TokenValidatorFunction
                                    .Handler;
                        }
                        else
                        {
                            pluginFunctions.ValidateToken = authorizationHeader => Task.FromResult(
                                ((Func<string, ClaimsPrincipal>) httpFunctionDefinition.TokenValidatorFunction
                                    .Handler)(authorizationHeader)
                            );
                        }

                        if (claimsMappings == null)
                        {
                            pluginFunctions.BindClaims = (principal, command) => Task.FromResult(command);
                        }
                        else
                        {
                            var mapperFunc = ImmutableCommandClaimsMapperBuilder.Build(httpFunctionDefinition, claimsMappings);
                            pluginFunctions.BindClaims = (principal, command) => Task.FromResult(mapperFunc(command, principal));
                        }
                    }
                    else
                    {
                        pluginFunctions.ValidateToken = async (authorizationHeader) =>
                        {
                            /*var tokenValidator = (FunctionMonkey.Abstractions.ITokenValidator)
                                ServiceProvider.GetService(httpFunctionDefinition
                                    .TokenValidatorType);*/
                            ITokenValidator tokenValidator = ServiceProvider.GetService<ITokenValidator>();
                            ClaimsPrincipal principal = await tokenValidator.ValidateAsync(authorizationHeader);
                            
                            return principal;
                        };
                        
                        pluginFunctions.BindClaims = async (principal, command) =>
                        {
                            var claimsBinder = (FunctionMonkey.Abstractions.ICommandClaimsBinder)
                                ServiceProvider.GetService(
                                    typeof(FunctionMonkey.Abstractions.ICommandClaimsBinder));
                            var claimsBinderTask = claimsBinder.BindAsync(principal, command);
                            if (claimsBinderTask == null)
                            {
                                return claimsBinder.Bind(principal, command);
                            }
                            return await claimsBinderTask;
                        };
                    }

                    pluginFunctions.IsAuthorized = async (principal, httpVerb, requestUrl) =>
                    {
                        var claimsPrincipalAuthorization = (IClaimsPrincipalAuthorization)
                            ServiceProvider.GetService(httpFunctionDefinition.ClaimsPrincipalAuthorizationType);
                        return await claimsPrincipalAuthorization.IsAuthorized(principal, httpVerb, requestUrl);
                    };

                    if (HasResponseFunctions(httpFunctionDefinition))
                    {
                        if (httpFunctionDefinition.CreateValidationFailureResponseFunction != null)
                        {
                            pluginFunctions.CreateValidationFailureResponse =
                                (Func<object, object, Task<IActionResult>>)
                                httpFunctionDefinition.CreateValidationFailureResponseFunction.Handler;
                        }
                        else
                        {
                            pluginFunctions.CreateValidationFailureResponse = (cmd, vr) => null;
                        }

                        if (httpFunctionDefinition.CreateResponseForResultFunction != null)
                        {
                            pluginFunctions.CreateResponseForResult =
                                (Func<object, object, Task<IActionResult>>) httpFunctionDefinition
                                    .CreateResponseForResultFunction.Handler;
                        }
                        else
                        {
                            pluginFunctions.CreateResponseForResult = (cmd, result) => null;
                        }
                    
                        if (httpFunctionDefinition.CreateResponseFunction != null)
                        {
                            pluginFunctions.CreateResponse =
                                (Func<object, Task<IActionResult>>) httpFunctionDefinition.CreateResponseFunction.Handler;
                        }
                        else
                        {
                            pluginFunctions.CreateResponse = cmd => null;
                        }
                    
                        if (httpFunctionDefinition.CreateResponseFromExceptionFunction != null)
                        {
                            pluginFunctions.CreateResponseFromException =
                                (Func<object, Exception, Task<IActionResult>>)httpFunctionDefinition.CreateResponseFromExceptionFunction.Handler;
                        }
                        else
                        {
                            pluginFunctions.CreateResponseFromException = (cmd, ex) => null;
                        }
                    }
                    else if (httpFunctionDefinition.HttpResponseHandlerType != null)
                    {
                        pluginFunctions.CreateValidationFailureResponse = (command, validationResult) =>
                        {
                            var responseHandler =
                                (IHttpResponseHandler) ServiceProvider.GetService(
                                    httpFunctionDefinition.HttpResponseHandlerType);
                            return responseHandler.CreateValidationFailureResponse(command, (ValidationResult)validationResult);
                        };
                        
                        pluginFunctions.CreateResponseForResult = (command, result) =>
                        {
                            var responseHandler =
                                (IHttpResponseHandler) ServiceProvider.GetService(
                                    httpFunctionDefinition.HttpResponseHandlerType);
                            return responseHandler.CreateResponse(command, result);
                        };
                        pluginFunctions.CreateResponse = command =>
                        {
                            var responseHandler =
                                (IHttpResponseHandler) ServiceProvider.GetService(
                                    httpFunctionDefinition.HttpResponseHandlerType);
                            return responseHandler.CreateResponse(command);
                        };
                        pluginFunctions.CreateResponseFromException = (command, exception) =>
                        {
                            var responseHandler =
                                (IHttpResponseHandler) ServiceProvider.GetService(
                                    httpFunctionDefinition.HttpResponseHandlerType);
                            return responseHandler.CreateResponseFromException(command, exception);
                        };
                    }
                    else
                    {
                        pluginFunctions.CreateValidationFailureResponse = (cmd, vr) => null;
                        pluginFunctions.CreateResponseForResult = (cmd, result) => null;
                        pluginFunctions.CreateResponse = cmd => null;
                        pluginFunctions.CreateResponseFromException = (cmd, ex) => null;
                    }

                    
                    
                    
                };
                    
                PluginFunctions.Add(functionDefinition.Name, pluginFunctions);
                
            }
        }
        
        private static bool HasResponseFunctions(HttpFunctionDefinition httpFunctionDefinition)
        {
            return httpFunctionDefinition.CreateValidationFailureResponseFunction != null ||
                   httpFunctionDefinition.CreateResponseForResultFunction != null ||
                   httpFunctionDefinition.CreateResponseFunction != null ||
                   httpFunctionDefinition.CreateResponseFromExceptionFunction != null;
        }

        private void RegisterCosmosDependencies(
            IReadOnlyCollection<AbstractFunctionDefinition> builderFunctionDefinitions)
        {
            HashSet<Type> types = new HashSet<Type>();
            foreach (AbstractFunctionDefinition abstractFunctionDefinition in builderFunctionDefinitions)
            {
                if (abstractFunctionDefinition is CosmosDbFunctionDefinition cosmosFunctionDefinition)
                {
                    if (cosmosFunctionDefinition.ErrorHandlerType != null)
                    {
                        types.Add(cosmosFunctionDefinition.ErrorHandlerType);
                    }
                }
            }

            foreach (Type claimsPrincipalAuthorizationType in types)
            {
                ServiceCollection.AddTransient(claimsPrincipalAuthorizationType);
            }
        }

        private void RegisterCoreDependencies(
            Type mediatorType,
            IReadOnlyCollection<AbstractFunctionDefinition> functionDefinitions,
            CompileTargetEnum target)
        {
            ServiceCollection.AddTransient(typeof(IMediatorDecorator), mediatorType);
            HashSet<Type> types = new HashSet<Type>();
            foreach (AbstractFunctionDefinition abstractFunctionDefinition in functionDefinitions)
            {
                types.Add(abstractFunctionDefinition.CommandDeserializerType);
                if (abstractFunctionDefinition.CommandTransformerType != null)
                {
                    types.Add(abstractFunctionDefinition.CommandTransformerType);
                }
            }
            foreach (Type type in types)
            {
                ServiceCollection.AddTransient(type);
            }

            if (target == CompileTargetEnum.AzureFunctions)
            {
                // Inject an ILogger that picks up the runtime provided logger
                ServiceCollection.AddTransient<ILogger>(sp => new FunctionLogger(this));
                ServiceCollection.AddTransient<ILoggerFactory>(sp => new FunctionLoggerFactory(this));
            }
        }

        private void RegisterHttpDependencies(
            IReadOnlyCollection<AbstractFunctionDefinition> builderFunctionDefinitions)
        {
            HashSet<Type> types = new HashSet<Type>();
            Type tokenValidatorType = null;
            foreach (AbstractFunctionDefinition abstractFunctionDefinition in builderFunctionDefinitions)
            {
                if (abstractFunctionDefinition is HttpFunctionDefinition httpFunctionDefinition)
                {
                    if (httpFunctionDefinition.ClaimsPrincipalAuthorizationType != null)
                    {
                        types.Add(httpFunctionDefinition.ClaimsPrincipalAuthorizationType);
                    }

                    if (httpFunctionDefinition.HttpResponseHandlerType != null)
                    {
                        types.Add(httpFunctionDefinition.HttpResponseHandlerType);
                    }

                    if (httpFunctionDefinition.TokenValidatorType != null)
                    {
                        if (tokenValidatorType != null &&
                            httpFunctionDefinition.TokenValidatorType != tokenValidatorType)
                        {
                            // this shouldn't happen as the builder interface doesn't allow it
                            // TODO: Remove TokenValidatorType from the HttpFunctionDefinition once we've completed
                            // the registration against ITokenValidator
                            throw new ConfigurationException("Only one token validator type can be set");
                        }
                        tokenValidatorType = httpFunctionDefinition.TokenValidatorType;
                    }
                }
            }

            if (tokenValidatorType != null)
            {
                ServiceCollection.AddTransient(typeof(ITokenValidator), tokenValidatorType);
            }
            
            foreach (Type claimsPrincipalAuthorizationType in types)
            {
                ServiceCollection.AddTransient(claimsPrincipalAuthorizationType);
            }
        }

        private void SetupAuthorization(
            FunctionHostBuilder builder,
            FunctionBuilder functionBuilder)
        {
            AuthorizationBuilder authorizationBuilder = (AuthorizationBuilder)builder.AuthorizationBuilder;

            // don't register the token validator type here - that gets passed down to the HTTP function
            // definitions to allow for function overrides and so is registered as part of HTTP dependencies
            if (authorizationBuilder.CustomClaimsBinderType == null)
            {
                ICommandClaimsBinder commandClaimsBinder = authorizationBuilder.ClaimsMappingBuilder.Build(
                    functionBuilder.GetHttpFunctionDefinitions().Select(x => x.CommandType).ToArray());
                ServiceCollection.AddSingleton(commandClaimsBinder);
            }
            else
            {
                ServiceCollection.AddTransient(typeof(ICommandClaimsBinder),
                    authorizationBuilder.CustomClaimsBinderType);
            }
        }

        private IFunctionAppConfiguration LocateConfiguration(Assembly functionAppConfigurationAssembly)
        {
            IFunctionAppConfiguration configuration = ConfigurationLocator.FindConfiguration(functionAppConfigurationAssembly);

            return configuration;
        }
        
        private IFunctionCompilerMetadata LocateFunctionCompilerMetadata(Assembly functionAppConfigurationAssembly)
        {
            IFunctionCompilerMetadata metadata = ConfigurationLocator.FindCompilerMetadata(functionAppConfigurationAssembly);
            return metadata;
        }

        private FunctionHostBuilder CreateBuilderFromConfiguration(
            ICommandRegistry commandRegistry,
            IFunctionAppConfiguration configuration)
        {
            FunctionHostBuilder builder = new FunctionHostBuilder(ServiceCollection, commandRegistry, true);
            configuration.Build(builder);
            DefaultMediatorSettings.SetDefaultsIfRequired(builder);
            RegisterCommandHandlersForCommandsWithNoAssociatedHandler(builder, commandRegistry);
            IMediatorResultTypeExtractor extractor = (IMediatorResultTypeExtractor)Activator.CreateInstance(builder.Options.MediatorResultTypeExtractor);
            
            new PostBuildPatcher(extractor).Patch(builder, "");
            return builder;
        }

        private void RegisterCommandHandlersForCommandsWithNoAssociatedHandler(FunctionHostBuilder builder, ICommandRegistry commandRegistry)
        {
            // TODO: We can improve this so that auto-registration is decoupled and can be provided by a mediator package
            if (builder.MediatorType != typeof(DefaultMediatorDecorator))
                return;
            
            // IN PROGRESS: This looks from the loaded set of assemblies and looks for a command handler for each command associated with a function.
            // If the handler is not already registered in the command registry then this registers it.
            IRegistrationCatalogue registrationCatalogue = (IRegistrationCatalogue) commandRegistry;
            HashSet<Type> registeredCommandTypes = new HashSet<Type>(registrationCatalogue.GetRegisteredCommands());

            Dictionary<Type, List<Type>> commandTypesToHandlerTypes = null;

            foreach (AbstractFunctionDefinition functionDefinition in builder.FunctionDefinitions)
            {
                if (registeredCommandTypes.Contains(functionDefinition.CommandType))
                {
                    continue;
                }

                if (commandTypesToHandlerTypes == null)
                {
                    commandTypesToHandlerTypes = HarvestCommandHandlers();
                }

                if (commandTypesToHandlerTypes.TryGetValue(functionDefinition.CommandType, out List<Type> handlerTypes))
                {
                    foreach (Type handlerType in handlerTypes)
                    {
                        commandRegistry.Register(handlerType);
                    }
                }
            }
        }

        private static Dictionary<Type, List<Type>> HarvestCommandHandlers()
        {
            Type baseCommandHandlerType = typeof(ICommandHandler);
            Dictionary<Type, List<Type>> commandTypesToHandlerTypes = new Dictionary<Type, List<Type>>();
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies)
            {
                foreach (Type candidateHandlerType in assembly.GetTypes())
                {
                    if (!candidateHandlerType.IsInterface && baseCommandHandlerType.IsAssignableFrom(candidateHandlerType))
                    {
                        Type[] genericArguments = candidateHandlerType.GetInterfaces()
                            .Where(i => i.IsGenericType &&
                                        (i.GetGenericTypeDefinition() == typeof(ICommandHandler<>) ||
                                         i.GetGenericTypeDefinition() == typeof(ICommandHandler<,>)))
                            .SelectMany(i => i.GetGenericArguments())
                            .ToArray();

                        if (genericArguments.Any())
                        {
                            Type commandArgumentType = genericArguments[0];
                            if (!commandTypesToHandlerTypes.TryGetValue(commandArgumentType,
                                out var handlerTypes))
                            {
                                handlerTypes = new List<Type>();
                                commandTypesToHandlerTypes[commandArgumentType] = handlerTypes;
                            }

                            handlerTypes.Add(candidateHandlerType);
                        }
                    }
                }
            }

            return commandTypesToHandlerTypes;
        }

        private void RegisterInternalImplementations()
        {
            // The below has been superceded by the singleton / dictionary registration
            //ServiceCollection.AddTransient<ICommandClaimsBinder, CommandClaimsBinder>();
            ServiceCollection.AddTransient<IContextSetter, ContextManager>();
            ServiceCollection.AddTransient<IContextProvider, ContextManager>();
        }

        private void RegisterTimerCommandFactories(
            IReadOnlyCollection<AbstractFunctionDefinition> functionDefinitions)
        {
            IReadOnlyCollection<TimerFunctionDefinition> timerFunctionDefinitions = functionDefinitions
                .Where(x => x is TimerFunctionDefinition)
                .Cast<TimerFunctionDefinition>()
                .Where(x => x.TimerCommandFactoryType != null)
                .ToArray();
            foreach (TimerFunctionDefinition timerFunctionDefinition in timerFunctionDefinitions)
            {
                Type interfaceType =
                    typeof(ITimerCommandFactory<>).MakeGenericType(timerFunctionDefinition.CommandType);
                ServiceCollection.AddTransient(interfaceType, timerFunctionDefinition.TimerCommandFactoryType);
            }
        }
    }
}
