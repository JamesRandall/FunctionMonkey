namespace FunctionMonkey.FSharp
open System
open System.Linq.Expressions
open System.Reflection
open System.Security.Claims
open System.Threading.Tasks
open System.Threading.Tasks
open FunctionMonkey.Commanding.Abstractions.Validation
open Microsoft.AspNetCore.Mvc
open Models
open BridgeFunctions
open InternalHelpers

module Configuration =
    type claimsMapper private () =
        static member inline shared (claimName, propertyName) =
            { claim = claimName ; mapper = Shared(propertyName) }
        static member command<'commandType, 'propertyType> (claimName, propertyExpression: Expression<Func<'commandType, 'propertyType>>) =
            let commandMapper = { commandType = typeof<'commandType> ; propertyInfo = (getPropertyInfo propertyExpression) }
            { claim = claimName ; mapper = Command (commandMapper) }
    
    type header private () =
        static member mapping(expr: Expression<Func<'a, 'propertyType>>, headerName: string) =
            let typedHeaderMapping : HeaderMapping<'a> =
                {
                    headerName = headerName
                    propertyName = (getPropertyInfo expr).Name
                }
            typedHeaderMapping
            
    type FunctionHandler<'a, 'b> =
        | AsyncHandler of ('a -> Async<'b>)
        | Handler of ('a -> 'b)
        | NoHandler
        
    let private connectionStringSettingNameFromString value =
        match value with | Some s -> ConnectionStringSettingName(s) | None -> DefaultConnectionStringSettingName
        
    let private defaultValue defaultValue optionalBool =
        match optionalBool with | Some s -> s | None -> defaultValue
        
    let private defaultFalse = defaultValue false
    
    let private defaultTrue = defaultValue true
    
    type azureFunction private() =
        static member http
            (
                (handler:FunctionHandler<'a,'b>),
                verb,
                ?subRoute,
                // common
                (?validator:'a -> 'validationResult),
                (?asyncExceptionResponseHandler:'a -> Exception -> Async<IActionResult>),
                (?asyncResponseHandler:'a -> 'b -> Async<IActionResult>),
                (?asyncValidationFailureResponseHandler:'a -> ValidationResult -> Async<IActionResult>),
                (?headerMappings:HeaderMapping<'a> list),
                ?authorizationMode,
                ?returnResponseBodyWithOutputBinding,
                ?serializer,
                ?deserializer
            ) : HttpFunction =
             {
                 coreAttributes = {
                    commandType = typeof<'a>
                    resultType = typeof<'b>
                    validator = validator |> bridgeWith createBridgedFunc
                    outputBinding = None
                    handler = new System.Func<'a, Task<'b>>(fun (cmd) -> match handler with
                                                                         | AsyncHandler h -> h(cmd) |> Async.StartAsTask
                                                                         | Handler h -> Task.FromResult(h(cmd))
                                                                         | NoHandler ->Task.FromResult((cmd :> obj) :?> 'b)
                                                        )
                    serializer = serializer |> bridgeWith createBridgedSerializer
                    deserializer = deserializer |> bridgeWith createBridgedDeserializer
                 }
                 // http specific
                 verbs = [verb]
                 route = (match subRoute with | Some r -> r | None -> "")
                 authorizationMode = authorizationMode
                 headerMappings = match headerMappings with
                                  | Some hm -> hm |> Seq.cast<IHeaderMapping> |> Seq.toList
                                  | None -> []
                 // functions                 
                 exceptionResponseHandler = asyncExceptionResponseHandler |> bridgeWith createBridgedExceptionResponseHandlerAsync
                 responseHandler = asyncResponseHandler |> bridgeWith createBridgedResponseHandlerAsync
                 validationFailureResponseHandler = asyncValidationFailureResponseHandler |> bridgeWith createBridgedValidationFailureResponseHandlerAsync
                 returnResponseBodyWithOutputBinding = match returnResponseBodyWithOutputBinding with | Some r -> r | _ -> false
             }
             
        static member storageQueue
            (
                (handler:FunctionHandler<'a,'b>),
                queueName,
                (?validator:'a -> 'validationResult),
                ?serializer,
                ?deserializer
            ) : StorageFunction =
             StorageQueue(
                  {
                     coreAttributes = {
                        commandType = typeof<'a>
                        resultType = typeof<'b>
                        validator = validator |> bridgeWith createBridgedFunc
                        outputBinding = None
                        handler = new System.Func<'a, Task<'b>>(
                                    fun (cmd) -> match handler with
                                                 | AsyncHandler h -> h(cmd) |> Async.StartAsTask
                                                 | Handler h -> Task.FromResult(h(cmd))
                                                 | NoHandler ->Task.FromResult((cmd :> obj) :?> 'b)
                                    )
                        serializer = match serializer with Some s -> s |> bridgeWith createBridgedSerializer | None -> null
                        deserializer = match deserializer with Some s -> s |> bridgeWith createBridgedDeserializer | None -> null
                     }
                     queueName = queueName
                     connectionStringSettingName = DefaultConnectionStringSettingName
                  }
             )
             
        static member blob
            (
                (handler:FunctionHandler<'a,'b>),
                blobPath,
                (?validator:'a -> 'validationResult),
                ?serializer,
                ?deserializer
            ) : StorageFunction =
             Blob(
                  {
                     coreAttributes = {
                        commandType = typeof<'a>
                        resultType = typeof<'b>
                        validator = validator |> bridgeWith createBridgedFunc
                        outputBinding = None
                        handler = new System.Func<'a, Task<'b>>(
                                    fun (cmd) -> match handler with
                                                 | AsyncHandler h -> h(cmd) |> Async.StartAsTask
                                                 | Handler h -> Task.FromResult(h(cmd))
                                                 | NoHandler ->Task.FromResult((cmd :> obj) :?> 'b)
                                    )
                        serializer = match serializer with Some s -> s |> bridgeWith createBridgedSerializer | None -> null
                        deserializer = match deserializer with Some s -> s |> bridgeWith createBridgedDeserializer | None -> null
                     }
                     blobPath = blobPath
                     connectionStringSettingName = DefaultConnectionStringSettingName
                  }
             )
             
        static member serviceBusQueue
            (
                (handler:FunctionHandler<'a,'b>),
                queueName,
                (?validator:'a -> 'validationResult),
                ?sessionIdEnabled,
                ?serializer,
                ?deserializer
            ) : ServiceBusFunction =
             Queue(
                  {
                     coreAttributes = {
                        commandType = typeof<'a>
                        resultType = typeof<'b>
                        validator = validator |> bridgeWith createBridgedFunc
                        outputBinding = None
                        handler = new System.Func<'a, Task<'b>>(
                                    fun (cmd) -> match handler with
                                                 | AsyncHandler h -> h(cmd) |> Async.StartAsTask
                                                 | Handler h -> Task.FromResult(h(cmd))
                                                 | NoHandler ->Task.FromResult((cmd :> obj) :?> 'b)
                                    )
                        serializer = match serializer with Some s -> s |> bridgeWith createBridgedSerializer | None -> null
                        deserializer = match deserializer with Some s -> s |> bridgeWith createBridgedDeserializer | None -> null
                     }
                     queueName = queueName
                     connectionStringSettingName = DefaultConnectionStringSettingName
                     sessionIdEnabled = match sessionIdEnabled with | Some v -> v | None -> false
                  }
             )
             
        static member serviceBusSubscription
            (
                (handler:FunctionHandler<'a,'b>),
                topicName,
                (subscriptionName:string),
                (?validator:'a -> 'validationResult),
                ?sessionIdEnabled,
                ?serializer,
                ?deserializer
            ) : ServiceBusFunction =
             Subscription(
                 {
                     coreAttributes = {
                        commandType = typeof<'a>
                        resultType = typeof<'b>
                        validator = validator |> bridgeWith createBridgedFunc
                        outputBinding = None
                        handler = new System.Func<'a, Task<'b>>(
                                    fun (cmd) -> match handler with
                                                 | AsyncHandler h -> h(cmd) |> Async.StartAsTask
                                                 | Handler h -> Task.FromResult(h(cmd))
                                                 | NoHandler ->Task.FromResult((cmd :> obj) :?> 'b)
                                    )
                        serializer = match serializer with Some s -> s |> bridgeWith createBridgedSerializer | None -> null
                        deserializer = match deserializer with Some s -> s |> bridgeWith createBridgedDeserializer | None -> null
                     }
                     topicName = topicName
                     subscriptionName = subscriptionName
                     connectionStringSettingName = DefaultConnectionStringSettingName
                     sessionIdEnabled = match sessionIdEnabled with | Some v -> v | None -> false
                 }
             )
             
        static member timer
            (
                (handler:FunctionHandler<'a,'b>),
                cronExpression,
                ?serializer,
                ?deserializer
            ) : TimerFunction =
                {
                    coreAttributes = {
                        commandType = typeof<'a>
                        resultType = typeof<'b>
                        validator = null
                        outputBinding = None
                        handler = new System.Func<'a, Task<'b>>(
                                    fun (cmd) -> match handler with
                                                 | AsyncHandler h -> h(cmd) |> Async.StartAsTask
                                                 | Handler h -> Task.FromResult(h(cmd))
                                                 | NoHandler ->Task.FromResult((cmd :> obj) :?> 'b)
                                    )
                        serializer = match serializer with Some s -> s |> bridgeWith createBridgedSerializer | None -> null
                        deserializer = match deserializer with Some s -> s |> bridgeWith createBridgedDeserializer | None -> null
                    }
                    cronExpression = cronExpression
                }
                
        static member cosmosDb
            (
                (handler:FunctionHandler<'a,'b>),
                databaseName: string,
                collectionName: string,
                (?validator:'a -> 'validationResult),
                ?serializer,
                ?deserializer,
                ?leaseDatabaseName,
                ?leaseConnectionName,
                ?trackRemainingWork,
                ?createLeaseCollectionIfNotExists,
                ?startFromBeginning,
                ?convertToPascalCase,
                ?leaseCollectionPrefix,
                ?maxItemsPerInvocation,
                ?feedPollDelay,
                ?leaseAcquireInterval,
                ?leaseExpirationInterval,
                ?leaseRenewInterval,
                ?checkpointFrequency,
                ?leasesCollectionThroughput,
                ?remainingWorkCronExpression
            ) : CosmosDbFunction =
                {
                    databaseName = databaseName
                    collectionName = collectionName
                    coreAttributes = {
                        commandType = typeof<'a>
                        resultType = typeof<'b>
                        validator = validator |> bridgeWith createBridgedFunc
                        outputBinding = None
                        handler = new System.Func<'a, Task<'b>>(
                                    fun (cmd) -> match handler with
                                                 | AsyncHandler h -> h(cmd) |> Async.StartAsTask
                                                 | Handler h -> Task.FromResult(h(cmd))
                                                 | NoHandler ->Task.FromResult((cmd :> obj) :?> 'b)
                                    )
                        serializer = match serializer with Some s -> s |> bridgeWith createBridgedSerializer | None -> null
                        deserializer = match deserializer with Some s -> s |> bridgeWith createBridgedDeserializer | None -> null
                    }
                    leaseCollectionName = leaseConnectionName |> defaultValue "leases"
                    leaseDatabaseName =  leaseDatabaseName |> defaultValue databaseName
                    connectionStringSettingName = DefaultConnectionStringSettingName
                    trackRemainingWork = trackRemainingWork |> defaultFalse
                    createLeaseCollectionIfNotExists = createLeaseCollectionIfNotExists |> defaultFalse
                    startFromBeginning = startFromBeginning |> defaultFalse
                    convertToPascalCase = convertToPascalCase |> defaultFalse
                    leaseCollectionPrefix = leaseCollectionPrefix
                    maxItemsPerInvocation = maxItemsPerInvocation
                    feedPollDelay = feedPollDelay
                    leaseAcquireInterval = leaseAcquireInterval
                    leaseExpirationInterval = leaseExpirationInterval
                    leaseRenewInterval = leaseRenewInterval
                    checkpointFrequency = checkpointFrequency
                    leasesCollectionThroughput = leasesCollectionThroughput
                    remainingWorkCronExpression = remainingWorkCronExpression |> defaultValue "*/5 * * * * *"
                }
            
                        
    type FunctionAppConfigurationBuilder() =
        member __.Yield (_: 'a) : FunctionAppConfiguration =
            {
             defaultFunctionAppConfiguration with
                defaultSerializer = Some Serialization.modelSerializer |> bridgeWith createBridgedSerializer
            }
        member __.Run (configuration: FunctionAppConfiguration) =
            let configurationToCreate =
                match configuration.enableFunctionModules with
                | true ->
                    let callingAssembly = Assembly.GetCallingAssembly()
                    let backlinkPropertyInfo = gatherBacklinkPropertyInfo callingAssembly
                    let moduleFunctions = gatherModuleFunctions callingAssembly
                    { configuration with functions = (concatFunctions [configuration.functions] moduleFunctions)
                                         backlinkPropertyInfo = backlinkPropertyInfo
                    }
                | false -> configuration
            FunctionCompilerMetadata.create configurationToCreate
        
        // Use to prevent functions being scavenged from functions { } blocks in other modules.
        // Useful in unit test scenarios (of Function Monkey)
        [<CustomOperation("disableFunctionModules")>]
        member this.disableFunctionModules(configuration:FunctionAppConfiguration) =
            { configuration with enableFunctionModules = false }
            
            
        [<CustomOperation("httpExceptionResponseHandler")>]
        member this.httpExceptionResponseHandler(configuration:FunctionAppConfiguration,
                                                 handler:('a -> Exception -> Async<IActionResult>)) =
            {
                configuration with defaultHttpResponseHandlers = {
                                configuration.defaultHttpResponseHandlers with exceptionResponseHandler =
                                                                                (Some handler)
                                                                                |> bridgeWith createBridgedExceptionResponseHandlerAsync
                }
            }
            
        [<CustomOperation("httpResponseHandler")>]
        member this.httpResponseHandler(configuration:FunctionAppConfiguration,
                                        handler:('a -> 'b -> Async<IActionResult>)) =
            {
                configuration with defaultHttpResponseHandlers = {
                                configuration.defaultHttpResponseHandlers with responseHandler =
                                                                                (Some handler)
                                                                                |> bridgeWith createBridgedResponseHandlerAsync
                }
            }
                    
        [<CustomOperation("outputSourcePath")>]
        member this.outputSourcePath(configuration:FunctionAppConfiguration, path) =
            { configuration with diagnostics = { configuration.diagnostics with outputSourcePath = OutputAuthoredSource.Path(path) } }
        
        // Authorization
        [<CustomOperation("defaultAuthorizationMode")>]
        member this.defaultAuthorizationMode(configuration: FunctionAppConfiguration, mode) =
            { configuration with authorization = { configuration.authorization with defaultAuthorizationMode = mode } }
        
        [<CustomOperation("defaultAuthorizationHeader")>]    
        member this.defaultAuthorizationHeader(configuration: FunctionAppConfiguration, header) =
            { configuration with authorization = { configuration.authorization with defaultAuthorizationHeader = header } }
        
        [<CustomOperation("isValid")>]
        member this.isValid(configuration:FunctionAppConfiguration, isValid:'validationResult -> bool) =
            { configuration with isValidHandler = Some isValid |> bridgeWith createBridgedFunc }            
        
        [<CustomOperation("tokenValidatorAsync")>]
        member this.tokenValidatorAsync(configuration:FunctionAppConfiguration, validator:string -> Async<ClaimsPrincipal>) =
            { configuration
                with authorization = {
                    configuration.authorization
                        with tokenValidator = new System.Func<string, Task<ClaimsPrincipal>>(fun t -> validator(t) |> Async.StartAsTask)
                }
            }
            
        [<CustomOperation("openApi")>]
        member this.openApi(configuration: FunctionAppConfiguration, title: string, version: string) =
            { configuration with openApi = Some (match configuration.openApi with
                                                 | Some c -> { c with title = title ; version = version }
                                                 | None -> { title = title ; version = version; userInterfaceEndpoint = None ; servers = []; outputPath = None } 
                                                )
            }
            
        [<CustomOperation("outputOpenApiPath")>]
        member this.outputOpenApiPath(configuration: FunctionAppConfiguration, path: string) =
            { configuration with openApi = Some (match configuration.openApi with
                                                 | Some c -> { c with outputPath = Some path }
                                                 | None -> { title = "Api" ; version = "1.0.0"; outputPath = Some path ; servers = []; userInterfaceEndpoint = None }
                                                )
            }
           
        [<CustomOperation("openApiUserInterface")>]
        member this.openApiUserInterface(configuration: FunctionAppConfiguration, endpoint: string) =
            { configuration with openApi = Some (match configuration.openApi with
                                                 | Some c -> { c with userInterfaceEndpoint = Some endpoint }
                                                 | None -> { title = "Api" ; version = "1.0.0"; userInterfaceEndpoint = Some endpoint ; servers = []; outputPath = None }
                                                )
            }
            
        [<CustomOperation("tokenValidator")>]
        member this.tokenValidator(configuration:FunctionAppConfiguration, validator:string -> ClaimsPrincipal) =
            { configuration
                with authorization = {
                    configuration.authorization
                        with tokenValidator = new System.Func<string, ClaimsPrincipal>(fun t -> validator(t))
                }
            }
        
        [<CustomOperation("claimsMappings")>]    
        member this.claimsMappings(configuration: FunctionAppConfiguration, claimsMappings) =
            { configuration
                with authorization = {
                    configuration.authorization with claimsMappings = claimsMappings
                }
            }
            
        [<CustomOperation("defaultSerializer")>]
        member this.defaultSerializer(configuration: FunctionAppConfiguration, serializer) =
            {
                configuration with defaultSerializer = Some serializer |> bridgeWith createBridgedSerializer
            }
            
        [<CustomOperation("defaultDeserializer")>]
        member this.defaultDeserializer(configuration: FunctionAppConfiguration, serializer) =
            {
                configuration with defaultDeserializer = Some serializer |> bridgeWith createBridgedDeserializer
            }
        
        // Functions
        [<CustomOperation("httpRoute")>]
        member this.httpRoute(configuration:FunctionAppConfiguration, prefix, httpFunctions) =
            { configuration
                with functions = {
                    configuration.functions
                        with httpFunctions = httpFunctions
                            |> List.map (fun f -> { f with route = prefix + f.route })
                            |> List.append configuration.functions.httpFunctions
                }
            }
        
        [<CustomOperation("serviceBus")>]    
        member this.serviceBus(configuration, connectionStringSettingName, serviceBusFunctions) =
            { configuration
                with functions = {
                    configuration.functions
                        with serviceBusFunctions = serviceBusFunctions
                            |> List.map (fun f -> match f with
                                                  | Queue q -> Queue({ q with connectionStringSettingName = connectionStringSettingName })
                                                  | Subscription s -> Subscription({ s with connectionStringSettingName = connectionStringSettingName })
                                        )
                            |> List.append configuration.functions.serviceBusFunctions
                }
            }
            
        [<CustomOperation("storage")>]
        member this.storage(configuration, connectionStringSettingName, storageFunctions) =
            { configuration
                with functions = {
                    configuration.functions
                        with storageFunctions = storageFunctions
                            |> List.map (fun f -> match f with
                                                  | StorageQueue q -> StorageQueue({ q with connectionStringSettingName = connectionStringSettingName })
                                                  | Blob b -> Blob({ b with connectionStringSettingName = connectionStringSettingName })
                                        )
                            |> List.append configuration.functions.storageFunctions
                }
            }
            
        [<CustomOperation("timers")>]
        member this.timers(configuration, timerFunctions) =
            { configuration
                with functions = {
                    configuration.functions
                        with timerFunctions = timerFunctions
                            |> List.append configuration.functions.timerFunctions
                }  
            }
        
        [<CustomOperation("cosmosDb")>]    
        member this.cosmosDb(configuration, connectionStringSettingName, cosmosDbFunctions) =
            { configuration
                with functions = {
                    configuration.functions
                        with cosmosDbFunctions = cosmosDbFunctions
                            |> List.append configuration.functions.cosmosDbFunctions
                            |> List.map(fun c -> { c with connectionStringSettingName = connectionStringSettingName })
                }  
            }
        
    let functionApp = FunctionAppConfigurationBuilder()
    
    type FunctionsBuilder() =
        member __.Yield (_: 'a) : Functions = defaultFunctions
        
        member __.Run (functions: Functions) =
            functions
        
        [<CustomOperation("httpRoute")>]
        member this.httpRoute(functions:Functions, prefix, httpFunctions) =
            { functions
                with httpFunctions = httpFunctions
                    |> Seq.map (fun f -> { f with route = prefix + f.route })
                    |> Seq.append functions.httpFunctions
                    |> Seq.toList
            }
            
        [<CustomOperation("serviceBus")>]    
        member this.serviceBus(functions:Functions, connectionStringSettingName, serviceBusFunctions) =
            { functions
                with serviceBusFunctions = serviceBusFunctions
                    |> Seq.map (function
                                | Queue q -> Queue({ q with connectionStringSettingName = connectionStringSettingName })
                                | Subscription s -> Subscription({ s with connectionStringSettingName = connectionStringSettingName })
                               )
                    |> Seq.append functions.serviceBusFunctions
                    |> Seq.toList
            }
            
        [<CustomOperation("storage")>]    
        member this.storage(functions:Functions, connectionStringSettingName, storageFunctions) =
            { functions
                with storageFunctions = storageFunctions
                    |> Seq.map (function
                                | StorageQueue q -> StorageQueue({ q with connectionStringSettingName = connectionStringSettingName })
                                | Blob s -> Blob({ s with connectionStringSettingName = connectionStringSettingName })
                               )
                    |> Seq.append functions.storageFunctions
                    |> Seq.toList
            }
    
    let functions = FunctionsBuilder()
