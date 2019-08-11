namespace FunctionMonkey.FSharp
open System
open System.Net.Http
open FunctionMonkey.Abstractions
open FunctionMonkey.Abstractions.Builders
open FunctionMonkey.Abstractions.Builders.Model
open FunctionMonkey.Abstractions.Http
open FunctionMonkey.Commanding.Abstractions.Validation
open FunctionMonkey.Model
open Models

module FunctionCompilerMetadata =
    exception HttpVerbNotSupportedException
    let create configuration =
        let createHttpFunctionDefinition (configuration:FunctionAppConfiguration) httpFunction =
            let convertVerb verb =
                match verb with
                | Get -> HttpMethod.Get
                | Put -> HttpMethod.Put
                | Post -> HttpMethod.Post
                | Patch -> HttpMethod.Patch
                | Delete -> HttpMethod.Delete
                
            let httpFunctionDefinition =
                HttpFunctionDefinition(
                    httpFunction.commandType,
                    httpFunction.resultType,
                    Verbs = System.Collections.Generic.HashSet(httpFunction.verbs |> Seq.map convertVerb),
                    Authorization = new System.Nullable<AuthorizationTypeEnum>(configuration.authorization.defaultAuthorizationMode),
                    ValidatesToken = (configuration.authorization.defaultAuthorizationMode = AuthorizationTypeEnum.TokenValidation),
                    TokenHeader = configuration.authorization.defaultAuthorizationHeader,
                    ClaimsPrincipalAuthorizationType = null,
                    HeaderBindingConfiguration = null,
                    HttpResponseHandlerType = null,
                    IsValidationResult = (not (httpFunction.resultType = typedefof<unit>) && typedefof<ValidationResult>.IsAssignableFrom(httpFunction.resultType)),
                    IsStreamCommand = false,
                    TokenValidatorType = null
                )
            
            httpFunctionDefinition :> AbstractFunctionDefinition
        
        {
            outputAuthoredSourceFolder = NoSourceOutput
            openApiConfiguration = OpenApiConfiguration()
            functionDefinitions =
                [] |> 
                Seq.append (configuration.functions.httpFunctions |> Seq.map (fun f -> createHttpFunctionDefinition configuration f))
                |> Seq.toList
                
        } :> IFunctionCompilerMetadata
