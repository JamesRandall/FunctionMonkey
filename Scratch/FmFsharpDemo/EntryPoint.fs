namespace FmFsharpDemo
open FunctionMonkey.FSharp.Configuration
open FunctionMonkey.FSharp.Models

module EntryPointCopy =
    let validateToken (bearerToken:string) = bearerToken.Length > 0
    
    let getApiVersion () =
        "1.0.0"
                                    
    let app = functionApp {
        tokenValidator validateToken
        httpRoute "version" [
            azureFunction.http (getApiVersion, Get)
        ]
    }
                
