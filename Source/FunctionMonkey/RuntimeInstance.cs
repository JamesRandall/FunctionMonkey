using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
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
    
    public class PluginFunctions : AbstractPluginFunctions // where TCommand : ICommand
    {
        public Func<string, Task<ClaimsPrincipal>> ValidateToken { get; set; }

        public Func<ClaimsPrincipal, string, string, Task<bool>> IsAuthorized { get; set; }
        
        public Func<string, object> Deserialize { get; set; }
        
        public Func<object, bool, string> Serialize { get; set; }
        
        public Func<ClaimsPrincipal, object, Task<bool>> BindClaims { get; set; }
        
        public Func<object, Exception, Task<IActionResult>> CreateResponseFromException { get; set; }
        
        public Func<object, object, Task<IActionResult>> CreateResponseForResult { get; set; }
        
        public Func<object, Task<IActionResult>> CreateResponse { get; set; }
        
        public Func<object, ValidationResult, Task<IActionResult>> CreateValidationFailureResponse { get; set; }
        
        public Func<object, ValidationResult> Validate { get; set; }

        // This is a func cast to an object that, when set, will be used to execute the command instead of the
        // built in dispatcher
        public object Handler { get; set; }
    }
    
    public class RuntimeInstance
    {
        public IServiceProvider ServiceProvider { get; }

        private IServiceCollection ServiceCollection { get; }

        public AsyncLocal<ILogger> FunctionProvidedLogger { get;  }= new AsyncLocal<ILogger>(null);

        public Dictionary<string, PluginFunctions> PluginFunctions { get; } = new Dictionary<string, PluginFunctions>();
        
        //public IFunctionHostBuilder Builder { get; }
        public IReadOnlyCollection<AbstractFunctionDefinition> FunctionDefinitions { get; }

        public RuntimeInstance() : this(null, null, null)
        {
            
        }

        public RuntimeInstance(Assembly functionAppConfigurationAssembly,
            Action<IServiceCollection, ICommandRegistry> beforeServiceProviderBuild,
            Action<IServiceProvider, ICommandRegistry> afterServiceProviderBuild)
        {

            IContainerProvider containerProvider;
            
            // Find the configuration implementation and service collection
            IFunctionAppConfiguration configuration = LocateConfiguration(functionAppConfigurationAssembly);
            if (configuration != null)
            {
                containerProvider =
                    // ReSharper disable once SuspiciousTypeConversion.Global - externally provided
                    (configuration as IContainerProvider) ?? new DefaultContainerProvider();

                ServiceCollection = containerProvider.CreateServiceCollection();
            }
            else
            {
                containerProvider = new DefaultContainerProvider();
            }
            ServiceCollection = containerProvider.CreateServiceCollection();
            
            CommandingDependencyResolverAdapter adapter = new CommandingDependencyResolverAdapter(
                (fromType, toInstance) => ServiceCollection.AddSingleton(fromType, toInstance),
                (fromType, toType) => ServiceCollection.AddTransient(fromType, toType),
                (resolveType) => ServiceProvider.GetService(resolveType)
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
            if (configuration != null)
            {
                // Invoke the builder process
                builder = CreateBuilderFromConfiguration(commandRegistry, configuration);
                FunctionBuilder functionBuilder = (FunctionBuilder)builder.FunctionBuilder;
                FunctionDefinitions = builder.FunctionDefinitions;
                
                SetupAuthorization(builder, functionBuilder);
            }
            else
            {
                IFunctionCompilerMetadata functionCompilerMetadata = LocateFunctionCompilerMetadata(functionAppConfigurationAssembly);
                FunctionDefinitions = functionCompilerMetadata.FunctionDefinitions;
            }

            RegisterCoreDependencies(FunctionDefinitions);

            RegisterTimerCommandFactories(FunctionDefinitions);

            RegisterHttpDependencies(FunctionDefinitions);

            RegisterCosmosDependencies(FunctionDefinitions);

            CreatePluginFunctions(FunctionDefinitions);

            beforeServiceProviderBuild?.Invoke(ServiceCollection, commandRegistry);
            ServiceProvider = containerProvider.CreateServiceProvider(ServiceCollection);
            afterServiceProviderBuild?.Invoke(ServiceProvider, commandRegistry);

            builder?.ServiceProviderCreatedAction?.Invoke(ServiceProvider);
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

        private void CreatePluginFunctions(IReadOnlyCollection<AbstractFunctionDefinition> functionDefinitions)
        {
            foreach (AbstractFunctionDefinition functionDefinition in functionDefinitions)
            {
                PluginFunctions pluginFunctions = new PluginFunctions();
                
                pluginFunctions.Deserialize = (body) =>
                    CreateSerializer(functionDefinition).Deserialize(functionDefinition.CommandType, body);

                pluginFunctions.Serialize = (content, enforceSecurityProperties) =>
                    CreateSerializer(functionDefinition).Serialize(content, enforceSecurityProperties); 
                
                pluginFunctions.Handler = functionDefinition.FunctionHandler;
                
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
                    }
                    else
                    {
                        pluginFunctions.ValidateToken = async (authorizationHeader) =>
                        {
                            var tokenValidator = (FunctionMonkey.Abstractions.ITokenValidator)
                                ServiceProvider.GetService(httpFunctionDefinition
                                    .TokenValidatorType);
                            ClaimsPrincipal principal = await tokenValidator.ValidateAsync(authorizationHeader);
                            return principal;
                        };
                    }
                    
                    pluginFunctions.IsAuthorized = async (principal, httpVerb, requestUrl) =>
                    {
                        var claimsPrincipalAuthorization = (IClaimsPrincipalAuthorization)
                            ServiceProvider.GetService(httpFunctionDefinition.ClaimsPrincipalAuthorizationType);
                        return await claimsPrincipalAuthorization.IsAuthorized(principal, httpVerb, requestUrl);
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
                    pluginFunctions.CreateValidationFailureResponse = (command, validationResult) =>
                    {
                        var responseHandler =
                            (IHttpResponseHandler) ServiceProvider.GetService(
                                httpFunctionDefinition.HttpResponseHandlerType);
                        return responseHandler.CreateValidationFailureResponse((ICommand) command, validationResult);
                    };
                    pluginFunctions.CreateResponseForResult = (command, result) =>
                    {
                        var responseHandler =
                            (IHttpResponseHandler) ServiceProvider.GetService(
                                httpFunctionDefinition.HttpResponseHandlerType);
                        return responseHandler.CreateResponse((ICommand) command, result);
                    };
                    pluginFunctions.CreateResponse = command =>
                    {
                        var responseHandler =
                            (IHttpResponseHandler) ServiceProvider.GetService(
                                httpFunctionDefinition.HttpResponseHandlerType);
                        return responseHandler.CreateResponse((ICommand) command);
                    };
                    pluginFunctions.CreateResponseFromException = (command, exception) =>
                    {
                        var responseHandler =
                            (IHttpResponseHandler) ServiceProvider.GetService(
                                httpFunctionDefinition.HttpResponseHandlerType);
                        return responseHandler.CreateResponseFromException((ICommand) command, exception);
                    };
                    pluginFunctions.Validate = command =>
                    {
                        var validator = (FunctionMonkey.Abstractions.Validation.IValidator)
                            ServiceProvider.GetService(
                                typeof(FunctionMonkey.Abstractions.Validation.IValidator));
                        var validationResult = validator.Validate((ICommand) command);
                        return validationResult;
                    };
                };
                    
                PluginFunctions.Add(functionDefinition.Name, pluginFunctions);
                
            }
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
            IReadOnlyCollection<AbstractFunctionDefinition> functionDefinitions)
        {
            HashSet<Type> types = new HashSet<Type>();
            foreach (AbstractFunctionDefinition abstractFunctionDefinition in functionDefinitions)
            {
                types.Add(abstractFunctionDefinition.CommandDeserializerType);
            }
            foreach (Type claimsPrincipalAuthorizationType in types)
            {
                ServiceCollection.AddTransient(claimsPrincipalAuthorizationType);
            }

            // Inject an ILogger that picks up the runtime provided logger
            ServiceCollection.AddTransient<ILogger>(sp => new FunctionLogger(this));
        }

        private void RegisterHttpDependencies(IReadOnlyCollection<AbstractFunctionDefinition> builderFunctionDefinitions)
        {
            HashSet<Type> types = new HashSet<Type>();
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
                        types.Add(httpFunctionDefinition.TokenValidatorType);
                    }
                }
            }

            foreach (Type claimsPrincipalAuthorizationType in types)
            {
                ServiceCollection.AddTransient(claimsPrincipalAuthorizationType);
            }
        }

        private void SetupAuthorization(FunctionHostBuilder builder, FunctionBuilder functionBuilder)
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

        private FunctionHostBuilder CreateBuilderFromConfiguration(ICommandRegistry commandRegistry,
            IFunctionAppConfiguration configuration)
        {
            FunctionHostBuilder builder = new FunctionHostBuilder(ServiceCollection, commandRegistry, true);
            configuration.Build(builder);
            RegisterCommandHandlersForCommandsWithNoAssociatedHandler(builder, commandRegistry);
            new PostBuildPatcher().Patch(builder, "");
            return builder;
        }

        private void RegisterCommandHandlersForCommandsWithNoAssociatedHandler(FunctionHostBuilder builder, ICommandRegistry commandRegistry)
        {
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
            ServiceCollection.AddTransient<ICommandClaimsBinder, CommandClaimsBinder>();
            ServiceCollection.AddTransient<IContextSetter, ContextManager>();
            ServiceCollection.AddTransient<IContextProvider, ContextManager>();
        }

        private void RegisterTimerCommandFactories(IReadOnlyCollection<AbstractFunctionDefinition> functionDefinitions)
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
