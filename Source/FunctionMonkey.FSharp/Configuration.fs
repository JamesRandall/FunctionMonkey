namespace FunctionMonkey.FSharp
open System
open System.Linq.Expressions
open System.Linq.Expressions
open System.Linq.Expressions
open System.Reflection
open System.Reflection
open System.Security.Claims
open System.Security.Claims
open System.Threading.Tasks
open FunctionMonkey.Abstractions.Builders
open FunctionMonkey.Commanding.Abstractions.Validation
open FunctionMonkey.Commanding.Abstractions.Validation
open FunctionMonkey.Commanding.Abstractions.Validation
open Models

module Configuration =
    let bridgeWith creator func =
        func |> Option.map(creator) |> Option.defaultValue null
    
    let private defaultAuthorization = {
        defaultAuthorizationMode = Function
        defaultAuthorizationHeader = "Authorization"
        tokenValidator = null
        claimsMappings = []
    }
    
    let private defaultFunctions = {
        httpFunctions = []
        serviceBusFunctions = []
    }
    
    let private defaultDiagnostics = {
        outputSourcePath = NoSourceOutput
    }
    
    let private defaultFunctionAppConfiguration = {
        enableFunctionModules = true
        diagnostics = defaultDiagnostics
        authorization = defaultAuthorization
        functions = defaultFunctions
    }
    
    let createBridgedValidatorFunc (validator:'a -> ValidationError list) =
        new System.Func<obj, FunctionMonkey.Commanding.Abstractions.Validation.ValidationResult>(fun cmd ->
            let createBridgedValidationError (error:FunctionMonkey.FSharp.Models.ValidationError) =
                new FunctionMonkey.Commanding.Abstractions.Validation.ValidationError(
                    Severity = (match error.severity with
                                | Error -> SeverityEnum.Error
                                | Warning -> SeverityEnum.Warning
                                | Info -> SeverityEnum.Info),
                    ErrorCode = (match error.errorCode with Some c -> c | None -> null),
                    Property = (match error.property with Some p -> p | None -> null),
                    Message = (match error.message with Some m -> m | None -> null)
                )
            let result = validator (cmd :?> 'a)
            new FunctionMonkey.Commanding.Abstractions.Validation.ValidationResult(
                Errors = (result |> Seq.map createBridgedValidationError |> Seq.toList)
            )
        )
        
    let private gatherModuleFunctions (assembly:Assembly) =
        assembly.GetTypes()
        |> Seq.collect (
               fun t -> t.GetProperties(BindingFlags.Public + BindingFlags.Static)
                        |> Seq.filter(fun p  -> p.PropertyType = typedefof<Functions>)
           )
        |> Seq.map (fun p -> p.GetValue(null) :?> Functions)
        
    let private concatFunctions functionsListA functionsListB =
        {
            httpFunctions = functionsListA |> Seq.collect(fun f -> f.httpFunctions)
                            |> Seq.append (functionsListB |> Seq.collect(fun f -> f.httpFunctions))
                            |> Seq.toList
            serviceBusFunctions = functionsListA |> Seq.collect(fun f -> f.serviceBusFunctions)
                            |> Seq.append (functionsListB |> Seq.collect(fun f -> f.serviceBusFunctions))
                            |> Seq.toList
        }
        
    let private getPropertyInfo (expression:Expression<Func<'commandType, 'propertyType>>) =
        let memberExpression =
            if expression.Body :? UnaryExpression then
                let unaryExpression = expression.Body :?> UnaryExpression
                unaryExpression.Operand :?> MemberExpression
            else
                expression.Body :?> MemberExpression
            
        memberExpression.Member :?> PropertyInfo            
    
    type claimsMapper private () =
        static member inline shared (claimName, propertyName) =
            { claim = claimName ; mapper = Shared(propertyName) }
        static member command<'commandType, 'propertyType> (claimName, (propertyExpression: Expression<Func<'commandType, 'propertyType>>)) =
            let commandMapper = { commandType = typedefof<'commandType> ; propertyInfo = (getPropertyInfo propertyExpression) }
            { claim = claimName ; mapper = Command (commandMapper) }
    
    type azureFunction private() =
        static member inline http
            (
                (handler:'a -> 'b),
                verb,
                ?subRoute,
                // common
                (?validator:'a -> ValidationError list)
            ) =
             {
                 verbs = [verb]
                 route = (match subRoute with | Some r -> r | None -> "")                 
                 commandType = typedefof<'a>
                 resultType = typedefof<'b>
                 // functions
                 handler = new System.Func<'a, 'b>(fun (cmd) -> handler (cmd))
                 validator = validator |> bridgeWith createBridgedValidatorFunc
             }
                        
    type FunctionAppConfigurationBuilder() =
        member __.Yield (_: 'a) : FunctionAppConfiguration = defaultFunctionAppConfiguration
        member __.Run (configuration: FunctionAppConfiguration) =
            let configurationToCreate =
                match configuration.enableFunctionModules with
                | true ->
                    let moduleFunctions = gatherModuleFunctions (Assembly.GetCallingAssembly())
                    { configuration with functions = (concatFunctions [configuration.functions] moduleFunctions) }
                | false -> configuration
            FunctionCompilerMetadata.create configurationToCreate
        
        // Use to prevent functions being scavenged from functions { } blocks in other modules.
        // Useful in unit test scenarios (of Function Monkey)
        [<CustomOperation("disableFunctionModules")>]
        member this.disableFunctionModules(configuration:FunctionAppConfiguration) =
            { configuration with enableFunctionModules = false }
        
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
        
        [<CustomOperation("tokenValidatorAsync")>]
        member this.tokenValidatorAsync(configuration:FunctionAppConfiguration, validator:string -> Async<ClaimsPrincipal>) =
            { configuration
                with authorization = {
                    configuration.authorization
                        with tokenValidator = new System.Func<string, Task<ClaimsPrincipal>>(fun t -> validator(t) |> Async.StartAsTask)
                }
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
        
        // Functions
        [<CustomOperation("httpRoute")>]
        member this.httpRoute(configuration:FunctionAppConfiguration, prefix, httpFunctions) =
            { configuration
                with functions = {
                    configuration.functions
                        with httpFunctions = httpFunctions
                            |> Seq.map (fun f -> { f with route = prefix + f.route })
                            |> Seq.append configuration.functions.httpFunctions
                            |> Seq.toList
                }
            }
        
        [<CustomOperation("serviceBus")>]    
        member this.serviceBus(configuration, serviceBusConnectionStringSettingName, serviceBusFunctions) =
            { configuration
                with functions = {
                    configuration.functions
                        with serviceBusFunctions = serviceBusFunctions
                            |> Seq.map (fun f -> match f with
                                                 | Queue q -> Queue({ q with serviceBusConnectionStringSettiingName = serviceBusConnectionStringSettingName })
                                                 | Subscription s -> Subscription({ s with serviceBusConnectionStringSettiingName = serviceBusConnectionStringSettingName })
                                       )
                            |> Seq.append configuration.functions.serviceBusFunctions
                            |> Seq.toList
                }
            }
        
    let functionApp = FunctionAppConfigurationBuilder()
    
    type FunctionsBuilder() =
        member __.Yield (_: 'a) : Functions = defaultFunctions
        
        [<CustomOperation("httpRoute")>]
        member this.httpRoute(functions:Functions, prefix, httpFunctions) =
            { functions
                with httpFunctions = httpFunctions
                    |> Seq.map (fun f -> { f with route = prefix + f.route })
                    |> Seq.append functions.httpFunctions
                    |> Seq.toList
            }
    
    let functions = FunctionsBuilder()
