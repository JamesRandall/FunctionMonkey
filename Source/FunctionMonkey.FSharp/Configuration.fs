namespace FunctionMonkey.FSharp
open System

module Configuration =    
    type HttpVerb =
            | Get
            | Put
            | Post
            | Patch
            | Delete
            | Custom of string

    type HttpRoute =
        | Path of string
        | Unspecified
    
    type IAzureFunction =
        abstract commandType: Type
        abstract resultType: Type
        
    type IHttpAzureFunction =
        inherit IAzureFunction
        abstract verbs: HttpVerb list
        abstract route: HttpRoute
        
    type IFunctionHandler = interface end
    
    type Handler<'commandType, 'commandResult> =
        {        
            handler: 'commandType -> 'commandResult
        }
        interface IFunctionHandler
        

    type HttpFunction =
        {
            commandType: Type
            resultType: Type
            handler: IFunctionHandler
            verbs: HttpVerb list
            route: HttpRoute
        }
        (*interface IHttpAzureFunction with
            member i.commandType = i.commandType
            member i.resultType = i.resultType
            member i.verbs = i.verbs
            member i.route = i.route*)
    
    type FunctionAppConfiguration = {
        httpFunctions: HttpFunction list
        functions: IAzureFunction list        
    }
    
    let private defaultFunctionAppConfiguration = { functions = [] ; httpFunctions = [] }
    
    let private combineRoutes firstPart secondPart =
        match firstPart with
        | Unspecified -> secondPart
        | Path p -> match secondPart with
                    | Unspecified -> firstPart
                    | Path p2 -> Path(p + p2)
    
    type azureFunction private() =
        static member inline http<'commandType, 'commandResultType> (handler:'commandType -> 'commandResultType, verb, ?subRoute) =
             {
                 verbs = [verb]
                 route = (match subRoute with | Some r -> Path(r) | None -> Unspecified)
                 handler = { handler = handler }
                 commandType = typedefof<'commandType>
                 resultType = typedefof<'commandResultType>
             }
        static member inline http<'commandType> (handler:'commandType -> unit , verb, ?subRoute) =
            azureFunction.http<'commandType, unit> (handler, verb, ?subRoute = subRoute)
                        

    type FunctionAppConfigurationBuilder() =
        member __.Yield (item: 'a) : FunctionAppConfiguration = defaultFunctionAppConfiguration
        
        [<CustomOperation("httpRoute")>]
        member this.httpRoute(configurationBuilder:FunctionAppConfiguration, prefix, x) =
            { configurationBuilder
                with httpFunctions = x
                    |> Seq.map (fun f -> { f with route = (combineRoutes (Path(prefix)) f.route) })
                    |> Seq.append configurationBuilder.httpFunctions
                    |> Seq.toList
            }
            
        [<CustomOperation("start")>]
        member this.start (configurationBuilder: FunctionAppConfiguration) =
            0
        
    let functionApp = FunctionAppConfigurationBuilder()
    
    let createFunctionAppWith config =
        0