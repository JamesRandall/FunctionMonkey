namespace FunctionMonkey.FSharp
open AzureFromTheTrenches.Commanding.Abstractions
open System
open System.Collections.Generic
open System.Net.Http
open System.Reflection
open System.Text.RegularExpressions
open FunctionMonkey.Abstractions
open FunctionMonkey.Abstractions.Builders
open FunctionMonkey.Abstractions.Builders.Model
open FunctionMonkey.Abstractions.Extensions
open FunctionMonkey.Abstractions.Http
open FunctionMonkey.Commanding.Abstractions.Validation
open FunctionMonkey.Compiler.Core
open FunctionMonkey.Compiler.Core
open FunctionMonkey.Model
open FunctionMonkey.Model
open FunctionMonkey.Model.OutputBindings
open FunctionMonkey.Serialization
open Models

module internal FunctionCompilerMetadata =
    exception OnlyRecordTypesSupportedForCommandsException
    
    let private (|Match|_|) pattern input =
        let m = Regex.Match(input, pattern) in
        if m.Success then Some (List.tail [ for g in m.Groups -> g.Value ]) else None
        
    let private createBridgedFunction func =
        match func with
        | null -> null
        | _ -> new BridgedFunction(func)
    
    let create configuration =
       
        let patchOutputBindingConnectionString (abstractOutputBinding:AbstractOutputBinding) =
            let ensureIsSet defaultValue (existingSetting:string) =
                match String.IsNullOrWhiteSpace(existingSetting) with | true -> defaultValue | _ -> existingSetting 
            
            match abstractOutputBinding with
            | :? CosmosOutputBinding as c -> c.ConnectionStringSettingName <- c.ConnectionStringSettingName |> ensureIsSet configuration.defaultConnectionSettingNames.cosmosDb
            | :? ServiceBusQueueOutputBinding as s -> s.ConnectionStringSettingName <- s.ConnectionStringSettingName |> ensureIsSet configuration.defaultConnectionSettingNames.serviceBus
            | :? ServiceBusTopicOutputBinding as t -> t.ConnectionStringSettingName <- t.ConnectionStringSettingName |> ensureIsSet configuration.defaultConnectionSettingNames.serviceBus
            | :? StorageQueueOutputBinding as sq -> sq.ConnectionStringSettingName <- sq.ConnectionStringSettingName |> ensureIsSet configuration.defaultConnectionSettingNames.storage
            | :? StorageTableOutputBinding as st -> st.ConnectionStringSettingName <- st.ConnectionStringSettingName |> ensureIsSet configuration.defaultConnectionSettingNames.storage
            | :? SignalROutputBinding as sr -> sr.ConnectionStringSettingName <- sr.ConnectionStringSettingName |> ensureIsSet configuration.defaultConnectionSettingNames.storage
            | _ -> ()
            
            abstractOutputBinding
        
        let extractConstructorParametersForType (aType:Type) =
            let createParameter (cp:ParameterInfo) =
                ImmutableTypeConstructorParameter(
                    Name = cp.Name,
                    Type = cp.ParameterType,
                    IsFSharpOptionType = InternalHelpers.isFSharpOptionType cp.ParameterType,
                    FSharpOptionInnerTypeName = InternalHelpers.optionTypeInnerName cp.ParameterType,
                    FSharpOptionInnerTypeIsString = InternalHelpers.optionTypeInnerTypeIsString cp.ParameterType
                )
            let constructors = aType.GetConstructors()
            match constructors.Length with
            | 0 -> []
            | 1 -> aType.GetConstructors().[0].GetParameters() |> Seq.map createParameter |> Seq.toList
            | _ -> raise OnlyRecordTypesSupportedForCommandsException
        
        let extractConstructorParameters attributes =
            extractConstructorParametersForType attributes.commandType
            
        let getNamespace coreAttributes =
            (sprintf "%s.Functions" (coreAttributes.commandType.Assembly.GetName().Name.Replace("-", "_")))
            
        let nullableInt (value:int option) = match value with | Some v -> System.Nullable<int>(v) | None -> System.Nullable<int>()
        
        let nullableString (value:string option) = match value with | Some s -> s | None -> null
        
        let isFSharpList (pt:Type) =
            let result = pt.IsGenericType && pt.GetGenericTypeDefinition() = typedefof<_ list>
            result
            
        let createCosmosDbFunctionDefinition (configuration:FunctionAppConfiguration) (cosmosFunction:CosmosDbFunction) =
            CosmosDbFunctionDefinition(
                cosmosFunction.coreAttributes.commandType,
                cosmosFunction.coreAttributes.resultType,
                DatabaseName = cosmosFunction.databaseName,
                CollectionName = cosmosFunction.collectionName,
                ConnectionStringName=(match cosmosFunction.connectionStringSettingName with
                                      | DefaultConnectionStringSettingName -> configuration.defaultConnectionSettingNames.storage
                                      | ConnectionStringSettingName s -> s),
                LeaseConnectionStringName = "",
                IsDocumentCommand = true,
                IsDocumentBatchCommand = false,
                LeaseCollectionName = cosmosFunction.leaseCollectionName,
                LeaseDatabaseName = cosmosFunction.leaseCollectionName,
                ConvertToPascalCase = cosmosFunction.convertToPascalCase,
                CreateLeaseCollectionIfNotExists = cosmosFunction.createLeaseCollectionIfNotExists,
                StartFromBeginning = cosmosFunction.startFromBeginning,
                LeaseCollectionPrefix = (cosmosFunction.leaseCollectionPrefix |> nullableString),
                TrackRemainingWork = cosmosFunction.trackRemainingWork,
                MaxItemsPerInvocation = (cosmosFunction.maxItemsPerInvocation |> nullableInt),
                FeedPollDelay = (cosmosFunction.feedPollDelay |> nullableInt),
                LeaseAcquireInterval = (cosmosFunction.leaseAcquireInterval |> nullableInt),
                LeaseExpirationInterval = (cosmosFunction.leaseExpirationInterval |> nullableInt),
                LeaseRenewInterval = (cosmosFunction.leaseRenewInterval |> nullableInt),
                CheckpointFrequency = (cosmosFunction.checkpointFrequency |> nullableInt),
                LeasesCollectionThroughput = (cosmosFunction.leasesCollectionThroughput |> nullableInt)
            )
        
            
        let createStorageQueueFunctionDefinition (configuration:FunctionAppConfiguration) (storageFunction:StorageQueueFunction) =
            StorageQueueFunctionDefinition(
                storageFunction.coreAttributes.commandType,
                storageFunction.coreAttributes.resultType,
                QueueName = storageFunction.queueName,
                ConnectionStringName=(match storageFunction.connectionStringSettingName with
                                      | DefaultConnectionStringSettingName -> configuration.defaultConnectionSettingNames.storage
                                      | ConnectionStringSettingName s -> s),
                // common settings
                ImmutableTypeConstructorParameters = extractConstructorParameters storageFunction.coreAttributes,
                CommandDeserializerType = typeof<CamelCaseJsonSerializer>,
                IsUsingValidator = not (storageFunction.coreAttributes.validator = null),
                UsesImmutableTypes = true,
                FunctionHandler = storageFunction.coreAttributes.handler,
                ValidatorFunction = storageFunction.coreAttributes.validator,                
                IsValidFunction = configuration.isValidHandler,
                Namespace = (storageFunction.coreAttributes |> getNamespace) + "2",
                SerializeFunction = (match storageFunction.coreAttributes.serializer with | null -> configuration.defaultSerializer | _ -> storageFunction.coreAttributes.serializer),
                DeserializeFunction = (match storageFunction.coreAttributes.deserializer with | null -> configuration.defaultDeserializer | _ -> storageFunction.coreAttributes.deserializer),
                OutputBinding = (match storageFunction.coreAttributes.outputBinding with
                                  | Some s -> ((s :?> AbstractOutputBinding) |> patchOutputBindingConnectionString)
                                  | None -> null)
            )
            
        let createStorageBlobFunctionDefinition (configuration:FunctionAppConfiguration) (storageFunction:StorageBlobFunction) =
            BlobFunctionDefinition(
                storageFunction.coreAttributes.commandType,
                storageFunction.coreAttributes.resultType,
                BlobPath = storageFunction.blobPath,
                ConnectionStringName=(match storageFunction.connectionStringSettingName with
                                      | DefaultConnectionStringSettingName -> configuration.defaultConnectionSettingNames.storage
                                      | ConnectionStringSettingName s -> s),
                // common settings
                ImmutableTypeConstructorParameters = extractConstructorParameters storageFunction.coreAttributes,
                CommandDeserializerType = typeof<CamelCaseJsonSerializer>,
                IsUsingValidator = not (storageFunction.coreAttributes.validator = null),
                UsesImmutableTypes = true,
                FunctionHandler = storageFunction.coreAttributes.handler,
                ValidatorFunction = storageFunction.coreAttributes.validator,                
                IsValidFunction = configuration.isValidHandler,
                Namespace = (storageFunction.coreAttributes |> getNamespace) + "2",
                SerializeFunction = (match storageFunction.coreAttributes.serializer with | null -> configuration.defaultSerializer | _ -> storageFunction.coreAttributes.serializer),
                DeserializeFunction = (match storageFunction.coreAttributes.deserializer with | null -> configuration.defaultDeserializer | _ -> storageFunction.coreAttributes.deserializer),
                OutputBinding = (match storageFunction.coreAttributes.outputBinding with
                                  | Some s -> ((s :?> AbstractOutputBinding) |> patchOutputBindingConnectionString)
                                  | None -> null)
            )
        
        let createTimerFunctionDefinition (configuration:FunctionAppConfiguration) (timerFunction:TimerFunction) =
            TimerFunctionDefinition(
                timerFunction.coreAttributes.commandType,
                timerFunction.coreAttributes.resultType,
                CronExpression = timerFunction.cronExpression,
                // common settings
                ImmutableTypeConstructorParameters = extractConstructorParameters timerFunction.coreAttributes,
                CommandDeserializerType = typeof<CamelCaseJsonSerializer>,
                IsUsingValidator = not (timerFunction.coreAttributes.validator = null),
                UsesImmutableTypes = true,
                FunctionHandler = timerFunction.coreAttributes.handler,
                ValidatorFunction = timerFunction.coreAttributes.validator,                
                IsValidFunction = configuration.isValidHandler,
                Namespace = (timerFunction.coreAttributes |> getNamespace) + "2",
                SerializeFunction = (match timerFunction.coreAttributes.serializer with | null -> configuration.defaultSerializer | _ -> timerFunction.coreAttributes.serializer),
                DeserializeFunction = (match timerFunction.coreAttributes.deserializer with | null -> configuration.defaultDeserializer | _ -> timerFunction.coreAttributes.deserializer),
                OutputBinding = (match timerFunction.coreAttributes.outputBinding with
                                  | Some s -> ((s :?> AbstractOutputBinding) |> patchOutputBindingConnectionString)
                                  | None -> null)
            )
        
        let createServiceBusQueueFunctionDefinition (configuration:FunctionAppConfiguration) (sbqFunction:ServiceBusQueueFunction) =
            ServiceBusQueueFunctionDefinition(
                sbqFunction.coreAttributes.commandType,
                sbqFunction.coreAttributes.resultType,
                ConnectionStringName=(match sbqFunction.connectionStringSettingName with
                                      | DefaultConnectionStringSettingName -> configuration.defaultConnectionSettingNames.serviceBus
                                      | ConnectionStringSettingName s -> s),
                QueueName=sbqFunction.queueName,
                IsSessionEnabled=sbqFunction.sessionIdEnabled,
                // common settings
                ImmutableTypeConstructorParameters = extractConstructorParameters sbqFunction.coreAttributes,
                CommandDeserializerType = typeof<CamelCaseJsonSerializer>,
                IsUsingValidator = not (sbqFunction.coreAttributes.validator = null),
                UsesImmutableTypes = true,
                FunctionHandler = sbqFunction.coreAttributes.handler,
                ValidatorFunction = sbqFunction.coreAttributes.validator,                
                IsValidFunction = configuration.isValidHandler,
                Namespace = (sbqFunction.coreAttributes |> getNamespace) + "2",
                SerializeFunction = (match sbqFunction.coreAttributes.serializer with | null -> configuration.defaultSerializer | _ -> sbqFunction.coreAttributes.serializer),
                DeserializeFunction = (match sbqFunction.coreAttributes.deserializer with | null -> configuration.defaultDeserializer | _ -> sbqFunction.coreAttributes.deserializer),
                OutputBinding = (match sbqFunction.coreAttributes.outputBinding with
                                  | Some s -> ((s :?> AbstractOutputBinding) |> patchOutputBindingConnectionString)
                                  | None -> null)
            )
            
        let createServiceBusSubscriptionFunctionDefinition  (configuration:FunctionAppConfiguration) (sbsFunction:ServiceBusSubscriptionFunction) =
            ServiceBusSubscriptionFunctionDefinition(
                sbsFunction.coreAttributes.commandType,
                sbsFunction.coreAttributes.resultType,
                ConnectionStringName=(match sbsFunction.connectionStringSettingName with
                                      | DefaultConnectionStringSettingName -> configuration.defaultConnectionSettingNames.serviceBus
                                      | ConnectionStringSettingName s -> s),
                TopicName=sbsFunction.topicName,
                SubscriptionName=sbsFunction.subscriptionName,
                IsSessionEnabled=sbsFunction.sessionIdEnabled,
                // common settings
                ImmutableTypeConstructorParameters = extractConstructorParameters sbsFunction.coreAttributes,
                CommandDeserializerType = typeof<CamelCaseJsonSerializer>,
                IsUsingValidator = not (sbsFunction.coreAttributes.validator = null),
                UsesImmutableTypes = true,
                FunctionHandler = sbsFunction.coreAttributes.handler,
                ValidatorFunction = sbsFunction.coreAttributes.validator,                
                IsValidFunction = configuration.isValidHandler,
                Namespace = (sbsFunction.coreAttributes |> getNamespace),
                SerializeFunction = (match sbsFunction.coreAttributes.serializer with | null -> configuration.defaultSerializer | _ -> sbsFunction.coreAttributes.serializer),
                DeserializeFunction = (match sbsFunction.coreAttributes.deserializer with | null -> configuration.defaultDeserializer | _ -> sbsFunction.coreAttributes.deserializer),
                OutputBinding = (match sbsFunction.coreAttributes.outputBinding with
                                  | Some s -> ((s :?> AbstractOutputBinding) |> patchOutputBindingConnectionString)
                                  | None -> null)
            )
        
        let createHttpFunctionDefinition (configuration:FunctionAppConfiguration) (httpFunction:HttpFunction) =
            let convertVerb verb =
                match verb with
                | Get -> HttpMethod.Get
                | Put -> HttpMethod.Put
                | Post -> HttpMethod.Post
                | Patch -> HttpMethod.Patch
                | Delete -> HttpMethod.Delete
                
            let convertAuthorizationMode mode =
                match mode with
                | Anonymous -> AuthorizationTypeEnum.Anonymous
                | Token -> AuthorizationTypeEnum.TokenValidation
                | Function -> AuthorizationTypeEnum.Function
                
            let extractQueryParameters (routeParameters:HttpParameter list) (headerMappings:HeaderBindingConfiguration) =
                let isSupportedFSharpQueryParameterType (pt:Type) =
                    // TODO: In here we need to look for the F# collection types
                    (pt |> InternalHelpers.isFSharpOptionType) || (pt |> isFSharpList)
                
                let propertyIsPossibleQueryParameter (x:PropertyInfo) =
                    x.GetCustomAttribute<SecurityPropertyAttribute>() = null
                    && (x.PropertyType.IsSupportedCSharpQueryParameterType() || x.PropertyType |> isSupportedFSharpQueryParameterType)
                    && not(routeParameters |> Seq.exists (fun y -> y.Name = x.Name))
                
                let properties =
                    httpFunction
                        .coreAttributes
                        .commandType
                        .GetProperties(BindingFlags.Instance ||| BindingFlags.Public)
                let queryParameters =
                    properties
                        |> Seq.filter propertyIsPossibleQueryParameter
                        |> Seq.map (fun q -> HttpParameter(Name=q.Name,
                                                           Type=q.PropertyType,
                                                           IsOptional=(q.PropertyType.IsValueType || not(Nullable.GetUnderlyingType(q.PropertyType) = null)),
                                                           HasHeaderMapping=headerMappings.PropertyFromHeaderMappings.ContainsKey(q.Name),
                                                           IsFSharpList = isFSharpList q.PropertyType,
                                                           IsFSharpOptionType = InternalHelpers.isFSharpOptionType q.PropertyType,
                                                           FSharpOptionInnerTypeName = InternalHelpers.optionTypeInnerName q.PropertyType,
                                                           FSharpOptionInnerTypeIsString = InternalHelpers.optionTypeInnerTypeIsString q.PropertyType
                                                          )
                                   )
                        |> Seq.toList
                queryParameters
                    
                
            let extractRouteParameters () =
                let createRouteParameter (parameterName:string) =
                    let isOptional = parameterName.EndsWith("?")
                    let parts = parameterName.Split(':')
                    let routeParameterName = parts.[0].TrimEnd('?')
                    
                    let matchedProperty = httpFunction.coreAttributes.commandType.GetProperties() |> Seq.find (fun p -> p.Name.ToLower() = routeParameterName.ToLower())
                    let isPropertyNullable = matchedProperty.PropertyType.IsValueType && not (Nullable.GetUnderlyingType(matchedProperty.PropertyType) = null)
                    
                    let routeTypeName = match isOptional && isPropertyNullable with
                                        | true -> sprintf "{%s}?" (matchedProperty.PropertyType.EvaluateType())
                                        | false -> matchedProperty.PropertyType.EvaluateType()
                    
                    HttpParameter(
                                     Name = matchedProperty.Name,
                                     Type = matchedProperty.PropertyType,
                                     IsOptional = isOptional,
                                     IsNullableType = not (Nullable.GetUnderlyingType(matchedProperty.PropertyType) = null),
                                     IsFSharpList = isFSharpList matchedProperty.PropertyType,
                                     IsFSharpOptionType = InternalHelpers.isFSharpOptionType matchedProperty.PropertyType,
                                     FSharpOptionInnerTypeName = InternalHelpers.optionTypeInnerName matchedProperty.PropertyType,
                                     FSharpOptionInnerTypeIsString = InternalHelpers.optionTypeInnerTypeIsString matchedProperty.PropertyType,
                                     RouteName = routeParameterName,
                                     RouteTypeName = routeTypeName
                                 )
                
                Regex.Matches(httpFunction.route, "{(.*?)}")
                    |> Seq.map (fun m -> m.Groups.[1].Value |> createRouteParameter)
                    |> Seq.toList
                
            let resolvedAuthorizationMode = match httpFunction.authorizationMode with
                                            | Some a -> a
                                            | None -> configuration.authorization.defaultAuthorizationMode                
            let routeParameters = extractRouteParameters ()
            let headerBindingConfiguration =
                match httpFunction.headerMappings.Length > 0 with
                | true -> HeaderBindingConfiguration(
                           PropertyFromHeaderMappings =
                               Dictionary<string,string>(
                                   httpFunction.headerMappings |> Seq.map(fun h -> KeyValuePair(h.propertyName, h.headerName))                             
                               )     
                           )
                | false -> HeaderBindingConfiguration(PropertyFromHeaderMappings=Dictionary<string,string>())
            HttpFunctionDefinition(
                 httpFunction.coreAttributes.commandType,
                 httpFunction.coreAttributes.resultType,
                 Route = httpFunction.route,
                 RouteConfiguration = HttpRouteConfiguration(OpenApiName = null, // TODO: Populate this from real values
                                                             OpenApiDescription = null),
                 UsesImmutableTypes = true,
                 Verbs = System.Collections.Generic.HashSet(httpFunction.verbs |> Seq.map convertVerb),
                 Authorization = new System.Nullable<AuthorizationTypeEnum>(convertAuthorizationMode resolvedAuthorizationMode),
                 ValidatesToken = (resolvedAuthorizationMode = Token),
                 TokenHeader = configuration.authorization.defaultAuthorizationHeader,
                 ClaimsPrincipalAuthorizationType = null,
                 HeaderBindingConfiguration = headerBindingConfiguration,
                 HttpResponseHandlerType = null,
                 IsValidationResult = (not (httpFunction.coreAttributes.resultType = typeof<unit>) && typeof<ValidationResult>.IsAssignableFrom(httpFunction.coreAttributes.resultType)),
                 IsStreamCommand = false,
                 TokenValidatorType = null,
                 QueryParameters = (extractQueryParameters routeParameters headerBindingConfiguration),
                 RouteParameters = routeParameters,
                 ImmutableTypeConstructorParameters = extractConstructorParameters httpFunction.coreAttributes,
                 Namespace = (httpFunction.coreAttributes |> getNamespace),                 
                 CommandDeserializerType = typeof<CamelCaseJsonSerializer>,
                 IsUsingValidator = not (httpFunction.coreAttributes.validator = null),
                 OutputBinding = (match httpFunction.coreAttributes.outputBinding with
                                  | Some s -> ((s :?> AbstractOutputBinding) |> patchOutputBindingConnectionString)
                                  | None -> null),
                 // function handlers - common
                 FunctionHandler = httpFunction.coreAttributes.handler,
                 ValidatorFunction = httpFunction.coreAttributes.validator,
                 // function handlers - http specific
                 TokenValidatorFunction = (match configuration.authorization.tokenValidator with
                                          | null -> null
                                          | _ -> new BridgedFunction(configuration.authorization.tokenValidator)),
                 CreateResponseFromExceptionFunction = (match httpFunction.exceptionResponseHandler with
                                                       | null -> configuration.defaultHttpResponseHandlers.exceptionResponseHandler
                                                       | _ -> httpFunction.exceptionResponseHandler),
                 CreateResponseForResultFunction = (match httpFunction.responseHandler with
                                                    | null -> configuration.defaultHttpResponseHandlers.responseHandler
                                                    | _ -> httpFunction.responseHandler),
                 CreateValidationFailureResponseFunction = (match httpFunction.validationFailureResponseHandler with
                                                            | null -> configuration.defaultHttpResponseHandlers.validationFailureResponseHandler
                                                            | _ -> httpFunction.validationFailureResponseHandler),
                 IsValidFunction = configuration.isValidHandler,
                 // Added for Function Monkey and also needed to be added to the C#
                 ReturnResponseBodyWithOutputBinding = httpFunction.returnResponseBodyWithOutputBinding,
                 SerializeFunction = (match httpFunction.coreAttributes.serializer with | null -> configuration.defaultSerializer | _ -> httpFunction.coreAttributes.serializer),
                 DeserializeFunction = (match httpFunction.coreAttributes.deserializer with | null -> configuration.defaultDeserializer | _ -> httpFunction.coreAttributes.deserializer)
                 
            ) :> AbstractFunctionDefinition
        
        
        // form up and return the function compiler metadata
        
        {
            defaultConnectionSettingNames = configuration.defaultConnectionSettingNames
            claimsMappings = configuration.authorization.claimsMappings |> Seq.map (
                                fun m -> match m.mapper with
                                         | Shared s -> SharedClaimsMappingDefinition(ClaimName = m.claim, PropertyPath = s) :> AbstractClaimsMappingDefinition
                                         | Command c -> CommandPropertyClaimsMappingDefinition(ClaimName = m.claim, CommandType = c.commandType, PropertyInfo = c.propertyInfo) :> AbstractClaimsMappingDefinition
                                ) |> Seq.toList
            outputAuthoredSourceFolder = configuration.diagnostics.outputSourcePath
            openApiConfiguration = (match configuration.openApi with
                                    | Some o -> OpenApiConfiguration(Version=o.version,
                                                                     Title=o.title,
                                                                     UserInterfaceRoute=(match o.userInterfaceEndpoint with
                                                                                         | Some e -> e
                                                                                         | None -> null),
                                                                     Servers=o.servers,
                                                                     OutputPath=(match o.outputPath with | Some p -> p | None -> null)
                                                                    )
                                    | None -> null)
            functionDefinitions =
                [] 
                |> Seq.append (configuration.functions.httpFunctions
                               |> Seq.map (fun f -> createHttpFunctionDefinition configuration f)
                               |> Seq.cast<AbstractFunctionDefinition>)
                |> Seq.append (configuration.functions.serviceBusFunctions
                               |> Seq.map (function
                                   | Queue q ->
                                       q |> createServiceBusQueueFunctionDefinition configuration :> AbstractFunctionDefinition
                                   | Subscription s ->
                                       s |> createServiceBusSubscriptionFunctionDefinition configuration :>AbstractFunctionDefinition))
                |> Seq.append (configuration.functions.storageFunctions
                               |> Seq.map (function
                                   | StorageQueue q ->
                                       q |> createStorageQueueFunctionDefinition configuration :> AbstractFunctionDefinition 
                                   | Blob s ->
                                       s |> createStorageBlobFunctionDefinition configuration :> AbstractFunctionDefinition))
                |> Seq.append (configuration.functions.timerFunctions
                               |> Seq.map (fun f -> f |> createTimerFunctionDefinition configuration) |> Seq.cast<AbstractFunctionDefinition>)
                |> Seq.append(configuration.functions.cosmosDbFunctions
                              |> Seq.map (fun f -> f |> createCosmosDbFunctionDefinition configuration) |> Seq.cast<AbstractFunctionDefinition>)
                //|> Seq.mapi(fun index functionDefinition -> functionDefinition.Name <- sprintf "%s%d" functionDefinition.Name index ; functionDefinition)
                |> Seq.toList
            backlinkReferenceType = configuration.backlinkPropertyInfo.DeclaringType
            backlinkPropertyInfo = configuration.backlinkPropertyInfo
            compilerOptions = CompilerOptions(HttpTarget = CompileTargetEnum.AzureFunctions)
        } :> IFunctionCompilerMetadata
