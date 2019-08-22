namespace FunctionMonkey.FSharp
open System
open System.Net.Http
open System.Reflection
open System.Text.RegularExpressions
open FunctionMonkey.Abstractions
open FunctionMonkey.Abstractions.Builders
open FunctionMonkey.Abstractions.Builders.Model
open FunctionMonkey.Abstractions.Extensions
open FunctionMonkey.Abstractions.Http
open FunctionMonkey.Commanding.Abstractions.Validation
open FunctionMonkey.Model
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
        let extractConstructorParameters func =
                let createParameter (cp:ParameterInfo) =
                    ImmutableTypeConstructorParameter(
                        Name = cp.Name,
                        Type = cp.ParameterType
                    )
                let constructors = func.commandType.GetConstructors()
                match constructors.Length with
                | 0 -> []
                | 1 -> func.commandType.GetConstructors().[0].GetParameters() |> Seq.map createParameter |> Seq.toList
                | _ -> raise OnlyRecordTypesSupportedForCommandsException
        
        let createHttpFunctionDefinition (configuration:FunctionAppConfiguration) httpFunction =
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
                
            let extractRouteParameters () =
                let createRouteParameter (parameterName:string) =
                    let isOptional = parameterName.EndsWith("?")
                    let parts = parameterName.Split(':')
                    let routeParameterName = parts.[0].TrimEnd('?')
                    
                    let matchedProperty = httpFunction.commandType.GetProperties() |> Seq.find (fun p -> p.Name.ToLower() = routeParameterName.ToLower())
                    let isPropertyNullable = matchedProperty.PropertyType.IsValueType && not (Nullable.GetUnderlyingType(matchedProperty.PropertyType) = null)
                    
                    let routeTypeName = match isOptional && isPropertyNullable with
                                        | true -> sprintf "{%s}?" (matchedProperty.PropertyType.EvaluateType())
                                        | false -> matchedProperty.PropertyType.EvaluateType()
                    
                    HttpParameter(
                                     Name = matchedProperty.Name,
                                     Type = matchedProperty.PropertyType,
                                     IsOptional = isOptional,
                                     IsNullableType = not (Nullable.GetUnderlyingType(matchedProperty.PropertyType) = null),
                                     RouteName = routeParameterName,
                                     RouteTypeName = routeTypeName
                                 )
                    
                match httpFunction.route with
                | Match "{(.*?)}" routeParams -> routeParams |> Seq.map createRouteParameter |> Seq.toList
                | _ -> []
                
                
            HttpFunctionDefinition(
                 httpFunction.commandType,
                 httpFunction.resultType,
                 Route = httpFunction.route,
                 UsesImmutableTypes = true,
                 Verbs = System.Collections.Generic.HashSet(httpFunction.verbs |> Seq.map convertVerb),
                 Authorization = new System.Nullable<AuthorizationTypeEnum>(convertAuthorizationMode configuration.authorization.defaultAuthorizationMode),
                 ValidatesToken = (configuration.authorization.defaultAuthorizationMode = Token),
                 TokenHeader = configuration.authorization.defaultAuthorizationHeader,
                 ClaimsPrincipalAuthorizationType = null,
                 HeaderBindingConfiguration = null,
                 HttpResponseHandlerType = null,
                 IsValidationResult = (not (httpFunction.resultType = typedefof<unit>) && typedefof<ValidationResult>.IsAssignableFrom(httpFunction.resultType)),
                 IsStreamCommand = false,
                 TokenValidatorType = null,
                 RouteParameters = extractRouteParameters (),
                 ImmutableTypeConstructorParameters = extractConstructorParameters httpFunction,
                 Namespace = (sprintf "%s.Functions" (httpFunction.commandType.Assembly.GetName().Name.Replace("-", "_"))),
                 CommandDeserializerType = typedefof<CamelCaseJsonSerializer>,
                 IsUsingValidator = not (httpFunction.validator = null),
                 OutputBinding = (match httpFunction.outputBinding with | Some s -> s :?> AbstractOutputBinding | None -> null),
                 // function handlers - common
                 FunctionHandler = httpFunction.handler,
                 ValidatorFunction = httpFunction.validator,
                 // function handlers - http specific
                 TokenValidatorFunction = (match configuration.authorization.tokenValidator with
                                          | null -> null
                                          | _ -> new BridgedFunction(configuration.authorization.tokenValidator)),
                 CreateResponseFromExceptionFunction = httpFunction.exceptionResponseHandler,
                 CreateResponseForResultFunction = httpFunction.responseHandler,
                 CreateValidationFailureResponseFunction = httpFunction.validationFailureResponseHandler,
                 IsValidFunction = configuration.isValidHandler
                 
            ) :> AbstractFunctionDefinition
        
        
        // form up and return the function compiler metadata
        
        {
            claimsMappings = configuration.authorization.claimsMappings |> Seq.map (
                                fun m -> match m.mapper with
                                         | Shared s -> SharedClaimsMappingDefinition(ClaimName = m.claim, PropertyPath = s) :> AbstractClaimsMappingDefinition
                                         | Command c -> CommandPropertyClaimsMappingDefinition(ClaimName = m.claim, CommandType = c.commandType, PropertyInfo = c.propertyInfo) :> AbstractClaimsMappingDefinition
                                ) |> Seq.toList
            outputAuthoredSourceFolder = configuration.diagnostics.outputSourcePath
            openApiConfiguration = OpenApiConfiguration()
            functionDefinitions =
                [] |> 
                Seq.append (configuration.functions.httpFunctions |> Seq.map (fun f -> createHttpFunctionDefinition configuration f))
                |> Seq.toList
                
        } :> IFunctionCompilerMetadata
