namespace FunctionMonkey.FSharp
open System
open System.Linq.Expressions
open System.Reflection
open System.Security.Claims
open System.Threading.Tasks
open FunctionMonkey.Commanding.Abstractions.Validation
open Microsoft.AspNetCore.Mvc
open Models
open BridgeFunctions
open Helpers

module Configuration =
    type claimsMapper private () =
        static member inline shared (claimName, propertyName) =
            { claim = claimName ; mapper = Shared(propertyName) }
        static member command<'commandType, 'propertyType> (claimName, (propertyExpression: Expression<Func<'commandType, 'propertyType>>)) =
            let commandMapper = { commandType = typedefof<'commandType> ; propertyInfo = (getPropertyInfo propertyExpression) }
            { claim = claimName ; mapper = Command (commandMapper) }
    
    type azureFunction private() =
        static member http
            (
                (handler:'a -> 'b),
                verb,
                ?subRoute,
                // common
                (?validator:'a -> ValidationError list),
                (?exceptionResponseHandlerAsync:'a -> Exception -> Async<IActionResult>),
                (?responseHandlerAsync:'a -> 'b -> Async<IActionResult>),
                (?validationFailureResponseHandlerAsync:'a -> ValidationResult -> Async<IActionResult>)
            ) =
             {
                 verbs = [verb]
                 route = (match subRoute with | Some r -> r | None -> "")                 
                 commandType = typedefof<'a>
                 resultType = typedefof<'b>
                 // functions
                 handler = new System.Func<'a, 'b>(fun (cmd) -> handler (cmd))
                 validator = validator |> bridgeWith createBridgedValidatorFunc
                 exceptionResponseHandler = exceptionResponseHandlerAsync |> bridgeWith createBridgedExceptionResponseHandlerAsync
                 responseHandler = responseHandlerAsync |> bridgeWith createBridgedResponseHandlerAsync
                 validationFailureResponseHandler = validationFailureResponseHandlerAsync |> bridgeWith createBridgedValidationFailureResponseHandlerAsync
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
