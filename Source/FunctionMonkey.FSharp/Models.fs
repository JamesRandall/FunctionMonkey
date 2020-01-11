namespace FunctionMonkey.FSharp
open System
open System.Collections.Generic
open System.Reflection
open FunctionMonkey.Abstractions
open FunctionMonkey.Abstractions.Builders.Model
open FunctionMonkey.Abstractions.Http
open FunctionMonkey.Compiler.Core
open FunctionMonkey.Model

module Models =
        
    type IOutputBindingTarget<'functionType> =
        abstract member setOutputBinding : obj -> 'functionType
        abstract member getOutputBinding : unit -> obj option
        abstract member resultType : Type
        abstract member getFunction : unit -> 'functionType
        
    type DefaultConnectionSettingNames =
        {
            cosmosDb: string
            serviceBus: string
            storage: string
            signalR: string
        }
        
    type ConnectionString =
        | DefaultConnectionStringSettingName
        | ConnectionStringSettingName of string
        
    type OpenApi =
        {
            title: string
            version: string
            userInterfaceEndpoint: string option
            servers: string list
            outputPath: string option
        }
        
    type BacklinkReference =
        | Disabled
        | AutoDetect
        | WithType of Type
    
    type OutputAuthoredSource =
        | Path of string
        | NoSourceOutput
        
    type CommandClaimsMapper =
        {
            commandType: Type
            propertyInfo: PropertyInfo
        }
        
    type ClaimsMapper =
        | Shared of string
        | Command of CommandClaimsMapper
        
    type ClaimsMapping =
        {
            claim: string            
            mapper: ClaimsMapper
        }
        
    type FunctionCompilerMetadata =
         {
             defaultConnectionSettingNames: DefaultConnectionSettingNames
             claimsMappings: AbstractClaimsMappingDefinition list
             functionDefinitions: AbstractFunctionDefinition list
             openApiConfiguration: OpenApiConfiguration
             outputAuthoredSourceFolder: OutputAuthoredSource
             backlinkReferenceType: Type
             backlinkPropertyInfo: PropertyInfo
             compilerOptions: CompilerOptions
         }
         interface IFunctionCompilerMetadata with
            member i.ClaimsMappings = i.claimsMappings :> System.Collections.Generic.IReadOnlyCollection<AbstractClaimsMappingDefinition>
            member i.FunctionDefinitions = i.functionDefinitions :> System.Collections.Generic.IReadOnlyCollection<AbstractFunctionDefinition>
            member i.OpenApiConfiguration = i.openApiConfiguration
            member i.OutputAuthoredSourceFolder = match i.outputAuthoredSourceFolder with | Path p -> p | NoSourceOutput -> null
            member i.BacklinkReferenceType = i.backlinkReferenceType
            member i.BacklinkPropertyInfo = i.backlinkPropertyInfo
            member i.CompilerOptions = i.compilerOptions
    
    type ValidationErrorSeverity =
        | ValidationError
        | ValidationWarning
        | ValidationInfo
        
    type ValidationError =
        {
            severity: ValidationErrorSeverity
            errorCode: string option
            property: string option
            message: string option
        }
        
    type AuthorizationMode =
        | Anonymous
        | Token
        | Function
    
    type HttpVerb =
            | Get
            | Put
            | Post
            | Patch
            | Delete    
    type HttpRoute =
        | Path of string
        | Unspecified
        
    type CoreFunctionAttributes =
        {
            commandType: Type
            resultType: Type
            handler: obj
            validator: BridgedFunction
            outputBinding: obj option
            serializer: BridgedFunction
            deserializer: BridgedFunction
        }
        
    type IHeaderMapping =
        abstract member headerName : string with get
        abstract member propertyName : string with get
        
    type HeaderMapping<'commandType> =
        {
            headerName: string
            propertyName: string
        }
        interface IHeaderMapping with
            member this.headerName with get() = this.headerName
            member this.propertyName with get() = this.propertyName
        
    type HttpFunction =
        {
            coreAttributes: CoreFunctionAttributes
            verbs: HttpVerb list
            route: string
            headerMappings: IHeaderMapping list
            exceptionResponseHandler: BridgedFunction
            responseHandler: BridgedFunction
            validationFailureResponseHandler: BridgedFunction
            authorizationMode: AuthorizationMode option
            returnResponseBodyWithOutputBinding: bool
        }
        interface IOutputBindingTarget<HttpFunction> with
            member this.setOutputBinding(binding: obj) = { this with coreAttributes= { this.coreAttributes with outputBinding = Some binding } }
            member this.getOutputBinding() = this.coreAttributes.outputBinding
            member this.getFunction() = this
            member this.resultType = this.coreAttributes.resultType
    
    type TimerFunction =
        {
            coreAttributes: CoreFunctionAttributes
            cronExpression: string
        }
        
    type CosmosDbFunction =
        {
            coreAttributes: CoreFunctionAttributes
            databaseName: string
            collectionName: string
            leaseCollectionName: string
            leaseDatabaseName: string
            connectionStringSettingName: ConnectionString
            trackRemainingWork: bool
            remainingWorkCronExpression: string
            createLeaseCollectionIfNotExists: bool
            startFromBeginning: bool
            convertToPascalCase: bool
            leaseCollectionPrefix: string option
            maxItemsPerInvocation: int option
            feedPollDelay: int option
            leaseAcquireInterval: int option
            leaseExpirationInterval: int option
            leaseRenewInterval: int option
            checkpointFrequency: int option
            leasesCollectionThroughput: int option
        }
        interface IOutputBindingTarget<CosmosDbFunction> with
            member this.setOutputBinding(binding: obj) = { this with coreAttributes= { this.coreAttributes with outputBinding = Some binding } }
            member this.getOutputBinding() = this.coreAttributes.outputBinding
            member this.getFunction() = this
            member this.resultType = this.coreAttributes.resultType
        
    type StorageQueueFunction =
        {
            coreAttributes: CoreFunctionAttributes
            connectionStringSettingName: ConnectionString
            queueName: string
        }
        interface IOutputBindingTarget<StorageQueueFunction> with
            member this.setOutputBinding(binding: obj) = { this with coreAttributes= { this.coreAttributes with outputBinding = Some binding } }
            member this.getOutputBinding() = this.coreAttributes.outputBinding
            member this.getFunction() = this
            member this.resultType = this.coreAttributes.resultType
    
    type StorageBlobFunction =
        {
            coreAttributes: CoreFunctionAttributes
            connectionStringSettingName: ConnectionString
            blobPath: string
        }
        interface IOutputBindingTarget<StorageBlobFunction> with
            member this.setOutputBinding(binding: obj) = { this with coreAttributes= { this.coreAttributes with outputBinding = Some binding } }
            member this.getOutputBinding() = this.coreAttributes.outputBinding
            member this.getFunction() = this
            member this.resultType = this.coreAttributes.resultType
        
    type StorageFunction =
        | StorageQueue of StorageQueueFunction
        | Blob of StorageBlobFunction
        member this.coreAttributes = match this with | StorageQueue q -> q.coreAttributes | Blob s -> s.coreAttributes
        interface IOutputBindingTarget<StorageFunction> with
            member this.setOutputBinding(binding: obj) =
                match this with
                | StorageQueue q -> StorageQueue({q with coreAttributes= { this.coreAttributes with outputBinding = Some binding } })
                | Blob q -> Blob({q with coreAttributes= { this.coreAttributes with outputBinding = Some binding } })                
            member this.getOutputBinding() = this.coreAttributes.outputBinding
            member this.getFunction() = this
            member this.resultType = this.coreAttributes.resultType
    
    type ServiceBusQueueFunction =
        {
            coreAttributes: CoreFunctionAttributes
            connectionStringSettingName: ConnectionString
            queueName: string
            sessionIdEnabled: bool
        }
        interface IOutputBindingTarget<ServiceBusQueueFunction> with
            member this.setOutputBinding(binding: obj) = { this with coreAttributes= { this.coreAttributes with outputBinding = Some binding } }
            member this.getOutputBinding() = this.coreAttributes.outputBinding
            member this.getFunction() = this
            member this.resultType = this.coreAttributes.resultType
    
    type ServiceBusSubscriptionFunction =
        {
            coreAttributes: CoreFunctionAttributes
            connectionStringSettingName: ConnectionString
            topicName: string
            subscriptionName: string
            sessionIdEnabled: bool
        }
        interface IOutputBindingTarget<ServiceBusSubscriptionFunction> with
            member this.setOutputBinding(binding: obj) = { this with coreAttributes= { this.coreAttributes with outputBinding = Some binding } }
            member this.getOutputBinding() = this.coreAttributes.outputBinding
            member this.getFunction() = this
            member this.resultType = this.coreAttributes.resultType
    
    type ServiceBusFunction =
        | Queue of ServiceBusQueueFunction
        | Subscription of ServiceBusSubscriptionFunction
        member this.coreAttributes = match this with | Queue q -> q.coreAttributes | Subscription s -> s.coreAttributes
        interface IOutputBindingTarget<ServiceBusFunction> with
            member this.setOutputBinding(binding: obj) =
                match this with
                | Queue q -> Queue({q with coreAttributes= { this.coreAttributes with outputBinding = Some binding } })
                | Subscription q -> Subscription({q with coreAttributes= { this.coreAttributes with outputBinding = Some binding } })                
            member this.getOutputBinding() = this.coreAttributes.outputBinding
            member this.getFunction() = this
            member this.resultType = this.coreAttributes.resultType
        
    type Authorization =
        {
            defaultAuthorizationMode: AuthorizationMode
            defaultAuthorizationHeader: string
            tokenValidator: obj
            claimsMappings: ClaimsMapping list
        }
    
    type Functions = {
        httpFunctions: HttpFunction list
        serviceBusFunctions: ServiceBusFunction list
        timerFunctions: TimerFunction list
        storageFunctions: StorageFunction list
        cosmosDbFunctions: CosmosDbFunction list
    }   
    
    type Diagnostics = {
        outputSourcePath: OutputAuthoredSource
    }
    
    type HttpResponseHandlers = {
        exceptionResponseHandler: BridgedFunction
        responseHandler: BridgedFunction
        validationFailureResponseHandler: BridgedFunction
    }
    
    type FunctionAppConfiguration = {
        openApi: OpenApi option
        defaultConnectionSettingNames: DefaultConnectionSettingNames
        defaultHttpResponseHandlers: HttpResponseHandlers
        enableFunctionModules: bool
        diagnostics: Diagnostics
        isValidHandler: BridgedFunction
        authorization: Authorization       
        functions: Functions
        backlinkPropertyInfo: PropertyInfo
        defaultSerializer: BridgedFunction
        defaultDeserializer: BridgedFunction
    }


    let private defaultAuthorization = {
        defaultAuthorizationMode = Function
        defaultAuthorizationHeader = "Authorization"
        tokenValidator = null
        claimsMappings = []
    }
    
    let defaultFunctions = {
        httpFunctions = []
        serviceBusFunctions = []
        timerFunctions = []
        storageFunctions = []
        cosmosDbFunctions = []
    }
    
    let private defaultDiagnostics = {
        outputSourcePath = NoSourceOutput
    }
    
    let defaultConnectionSettingNames = {
        cosmosDb = "cosmosConnectionString"
        storage = "storageConnectionString"
        serviceBus = "serviceBusConnectionString"
        signalR = "signalRConnectionString"
    }
    
    let defaultHttpResponseHandlers = {
        exceptionResponseHandler = null
        responseHandler = null
        validationFailureResponseHandler = null
    }
    
    let defaultFunctionAppConfiguration = {
        enableFunctionModules = true
        isValidHandler = null
        diagnostics = defaultDiagnostics
        defaultHttpResponseHandlers = defaultHttpResponseHandlers
        authorization = defaultAuthorization
        functions = defaultFunctions
        defaultConnectionSettingNames = defaultConnectionSettingNames
        openApi = None
        backlinkPropertyInfo = null
        defaultSerializer = null
        defaultDeserializer = null
    }
    
    
