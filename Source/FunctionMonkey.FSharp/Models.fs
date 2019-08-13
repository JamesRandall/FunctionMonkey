namespace FunctionMonkey.FSharp
open System
open FunctionMonkey.Abstractions
open FunctionMonkey.Abstractions.Builders
open FunctionMonkey.Abstractions.Builders.Model
open FunctionMonkey.Abstractions.Http

module Models =
    
    type IHandler = interface end
    
    type FunctionHandler<'command, 'result> =
        {
            func: Func<'command, 'result>
        }
        interface IHandler
    
    type OutputAuthoredSource =
        | Path of string
        | NoSourceOutput
        
    type FunctionCompilerMetadata =
         {
             functionDefinitions: AbstractFunctionDefinition list
             openApiConfiguration: OpenApiConfiguration
             outputAuthoredSourceFolder: OutputAuthoredSource
         }
         interface IFunctionCompilerMetadata with
            member i.FunctionDefinitions = i.functionDefinitions :> System.Collections.Generic.IReadOnlyCollection<AbstractFunctionDefinition>
            member i.OpenApiConfiguration = i.openApiConfiguration
            member i.OutputAuthoredSourceFolder = match i.outputAuthoredSourceFolder with | Path p -> p | NoSourceOutput -> null
        
    type HttpVerb =
            | Get
            | Put
            | Post
            | Patch
            | Delete
            //| Custom of string
    type HttpRoute =
        | Path of string
        | Unspecified
    type HttpFunction =
        {
            commandType: Type
            resultType: Type
            handler: obj
            verbs: HttpVerb list
            route: string
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
            defaultAuthorizationMode: AuthorizationTypeEnum
            defaultAuthorizationHeader: string
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

