namespace FunctionMonkey.FSharp
open FunctionMonkey.Abstractions.Builders
open Models

module Configuration =
    let private defaultAuthorization = {
        defaultAuthorizationMode = AuthorizationTypeEnum.Function
        defaultAuthorizationHeader = "Bearer"
    }
    
    let private defaultFunctions = {
        httpFunctions = []
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
            FunctionCompilerMetadata.create configuration
        
        [<CustomOperation("disableFunctionModules")>]
        member this.disableFunctionModules(configuration:FunctionAppConfiguration) =
            { configuration with enableFunctionModules = false }
        
        [<CustomOperation("outputSourcePath")>]
        member this.outputSourcePath(configuration:FunctionAppConfiguration, path) =
            { configuration with diagnostics = { configuration.diagnostics with outputSourcePath = OutputAuthoredSource.Path(path) } }
        
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
        
    let functionApp = FunctionAppConfigurationBuilder()
    
