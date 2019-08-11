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
    
    let private defaultFunctionAppConfiguration = {
        authorization = defaultAuthorization
        functions = defaultFunctions
    }
    
    let private combineRoutes firstPart secondPart =
        match firstPart with
        | Unspecified -> secondPart
        | Path p -> match secondPart with
                    | Unspecified -> firstPart
                    | Path p2 -> Path(p + p2)    
    
    type azureFunction private() =
        static member inline http ((handler:'a -> 'b), verb, ?subRoute) =
             (*let handlerType = handler.GetType()
             System.Console.WriteLine(handlerType.BaseType.BaseType.FullName)
             let fsharpFuncType = typedefof<Microsoft.FSharp.Core.FSharpFunc<_,_>>
             let handlerMethodInfo = match fsharpFuncType.IsAssignableFrom(handlerType) with
                                     | true -> handlerType.GetMethods().[0]
                                     | false -> raise (ConfigurationException(sprintf "Handler type has %s unexpected number of methods. Should have 1 but has %d" (handlerType.FullName) (handlerType.GetMethods().Length)))
             let commandType = match handlerMethodInfo.GetParameters().Length with
                               | 1 -> handlerMethodInfo.GetParameters().[0].ParameterType
                               | _ -> raise (ConfigurationException(sprintf "Handler type has %s unexpected number of parameters. Should have 1 but has %d" (handlerType.FullName) (handlerMethodInfo.GetParameters().Length)))
             let commandResultType = handlerMethodInfo.ReturnType*)
             {
                 verbs = [verb]
                 route = (match subRoute with | Some r -> Path(r) | None -> Unspecified)
                 handler = { handler = handler }
                 commandType = typedefof<'a>
                 resultType = typedefof<'b>
             }
                        
    type FunctionAppConfigurationBuilder() =
        member __.Yield (_: 'a) : FunctionAppConfiguration = defaultFunctionAppConfiguration
        member __.Run (configuration: FunctionAppConfiguration) =
            FunctionCompilerMetadata.create configuration
        
        [<CustomOperation("httpRoute")>]
        member this.httpRoute(configuration:FunctionAppConfiguration, prefix, httpFunctions) =
            { configuration
                with functions = {
                    configuration.functions
                        with httpFunctions = httpFunctions
                            |> Seq.map (fun f -> { f with route = (combineRoutes (Path(prefix)) f.route) })
                            |> Seq.append configuration.functions.httpFunctions
                            |> Seq.toList
                }
            }
        
    let functionApp = FunctionAppConfigurationBuilder()
    
