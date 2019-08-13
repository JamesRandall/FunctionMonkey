namespace FunctionMonkey.FSharp
open System.Reflection
open System.Threading.Tasks
open FunctionMonkey.Abstractions.Builders
open Models

module Configuration =
    let private defaultAuthorization = {
        defaultAuthorizationMode = AuthorizationTypeEnum.Function
        defaultAuthorizationHeader = "Bearer"
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
    
    type azureFunction private() =
        static member inline http ((handler:'a -> 'b), verb, ?subRoute) =
             {
                 verbs = [verb]
                 route = (match subRoute with | Some r -> r | None -> "")
                 handler = new System.Func<'a, 'b>(fun (cmd) -> handler (cmd))
                 commandType = typedefof<'a>
                 resultType = typedefof<'b>
             }
                        
    type FunctionAppConfigurationBuilder() =
        member __.Yield (_: 'a) : FunctionAppConfiguration = defaultFunctionAppConfiguration
        member __.Run (configuration: FunctionAppConfiguration) =
            let moduleFunctions = gatherModuleFunctions (Assembly.GetCallingAssembly())
            let configurationWithModuleFunctions = { configuration with functions = (concatFunctions [configuration.functions] moduleFunctions) }
            FunctionCompilerMetadata.create configurationWithModuleFunctions
        
        [<CustomOperation("disableFunctionModules")>]
        member this.disableFunctionModules(configuration:FunctionAppConfiguration) =
            { configuration with enableFunctionModules = false }
        
        [<CustomOperation("outputSourcePath")>]
        member this.outputSourcePath(configuration:FunctionAppConfiguration, path) =
            { configuration with diagnostics = { configuration.diagnostics with outputSourcePath = OutputAuthoredSource.Path(path) } }
            
        [<CustomOperation("tokenValidatorAsync")>]
        member this.tokenValidatorAsync(configuration:FunctionAppConfiguration, validator:string -> Async<bool>) =
            configuration
            
        [<CustomOperation("tokenValidator")>]
        member this.tokenValidator(configuration:FunctionAppConfiguration, validator:string -> bool) =
            configuration
        
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
