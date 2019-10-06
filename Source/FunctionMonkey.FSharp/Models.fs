namespace FunctionMonkey.FSharp
open System
open System.Collections.Generic
open System.Reflection
open FunctionMonkey.Abstractions
open FunctionMonkey.Abstractions.Builders.Model
open FunctionMonkey.Abstractions.Http
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
         }
         interface IFunctionCompilerMetadata with
            member i.ClaimsMappings = i.claimsMappings :> System.Collections.Generic.IReadOnlyCollection<AbstractClaimsMappingDefinition>
            member i.FunctionDefinitions = i.functionDefinitions :> System.Collections.Generic.IReadOnlyCollection<AbstractFunctionDefinition>
            member i.OpenApiConfiguration = i.openApiConfiguration
            member i.OutputAuthoredSourceFolder = match i.outputAuthoredSourceFolder with | Path p -> p | NoSourceOutput -> null
            member i.BacklinkReferenceType = i.backlinkReferenceType
            member i.BacklinkPropertyInfo = i.backlinkPropertyInfo
    
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
        }
        
    type HttpFunction =
        {
            coreAttributes: CoreFunctionAttributes
            verbs: HttpVerb list
            route: string
            exceptionResponseHandler: BridgedFunction
            responseHandler: BridgedFunction
            validationFailureResponseHandler: BridgedFunction
            authorizationMode: AuthorizationMode option
            returnResponseBodyWithOutputBinding: bool
            serializer: BridgedFunction
            deserializer: BridgedFunction
        }
        interface IOutputBindingTarget<HttpFunction> with
            member this.setOutputBinding(binding: obj) = { this with coreAttributes= { this.coreAttributes with outputBinding = Some binding } }
            member this.getOutputBinding() = this.coreAttributes.outputBinding
            member this.getFunction() = this
            member this.resultType = this.coreAttributes.resultType
        
    type ServiceBusQueueFunction =
        {
            coreAttributes: CoreFunctionAttributes
            connectionStringSettingName: ConnectionString
            queueName: string
            sessionIdEnabled: bool
            serializer: BridgedFunction
            deserializer: BridgedFunction
        }
    
    type ServiceBusSubscriptionFunction =
        {
            coreAttributes: CoreFunctionAttributes
            connectionStringSettingName: ConnectionString
            topicName: string
            subscriptionName: string
            sessionIdEnabled: bool
            serializer: BridgedFunction
            deserializer: BridgedFunction
        }
    
    type ServiceBusFunction =
        | Queue of ServiceBusQueueFunction
        | Subscription of ServiceBusSubscriptionFunction
        
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
    
    
