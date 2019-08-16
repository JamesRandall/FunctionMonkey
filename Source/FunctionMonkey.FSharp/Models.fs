namespace FunctionMonkey.FSharp
open System
open System.Reflection
open FunctionMonkey.Abstractions
open FunctionMonkey.Abstractions.Builders.Model
open FunctionMonkey.Abstractions.Http

module Models =
        
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
             claimsMappings: AbstractClaimsMappingDefinition list
             functionDefinitions: AbstractFunctionDefinition list
             openApiConfiguration: OpenApiConfiguration
             outputAuthoredSourceFolder: OutputAuthoredSource
         }
         interface IFunctionCompilerMetadata with
            member i.ClaimsMappings = i.claimsMappings :> System.Collections.Generic.IReadOnlyCollection<AbstractClaimsMappingDefinition>
            member i.FunctionDefinitions = i.functionDefinitions :> System.Collections.Generic.IReadOnlyCollection<AbstractFunctionDefinition>
            member i.OpenApiConfiguration = i.openApiConfiguration
            member i.OutputAuthoredSourceFolder = match i.outputAuthoredSourceFolder with | Path p -> p | NoSourceOutput -> null
    
    type ValidationErrorSeverity =
        | Error
        | Warning
        | Info
        
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
    type HttpFunction =
        {
            commandType: Type
            resultType: Type
            verbs: HttpVerb list
            route: string
            handler: obj
            validator: BridgedFunction
            exceptionResponseHandler: BridgedFunction
            responseHandler: BridgedFunction
            validationFailureResponseHandler: BridgedFunction
        }
        
    type ServiceBusQueueFunction = {
        serviceBusConnectionStringSettiingName: string
        queueName: string
        sessionIdEnabled: bool
    }
    
    type ServiceBusSubscriptionFunction = {
        serviceBusConnectionStringSettiingName: string
        topicName: string
        subscriptionName: string
        sessionIdEnabled: bool
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
    
    type FunctionAppConfiguration = {
        enableFunctionModules: bool
        diagnostics: Diagnostics
        authorization: Authorization       
        functions: Functions
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
    
    let defaultFunctionAppConfiguration = {
        enableFunctionModules = true
        diagnostics = defaultDiagnostics
        authorization = defaultAuthorization
        functions = defaultFunctions
    }
