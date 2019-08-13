namespace FmFsharpDemo
open System.Security.Claims
open FunctionMonkey.Abstractions.Builders
open FunctionMonkey.FSharp.Configuration
open FunctionMonkey.FSharp.Models

module EntryPointCopy =
    exception InvalidTokenException
    
    let validateToken (bearerToken:string) =
        match bearerToken.Length with
        | 0 -> raise InvalidTokenException
        | _ -> new ClaimsPrincipal(new ClaimsIdentity([new Claim("somevalue", "42")]))
    
    let getApiVersion () =
        "1.0.0"
                                    
    let app = functionApp {
        outputSourcePath "/Users/jamesrandall/code/authoredSource"
        defaultAuthorizationMode Token
        tokenValidator validateToken
        httpRoute "version" [
            azureFunction.http (getApiVersion, Get)
        ]
    }
                
