namespace FmFsharpDemo
open System.Security.Claims
open FunctionMonkey.Abstractions.Builders
open FunctionMonkey.FSharp.Configuration
open FunctionMonkey.FSharp.Configuration
open FunctionMonkey.FSharp.Models
open Orders

module EntryPointCopy =
    exception InvalidTokenException
    
    let validateToken (bearerToken:string) =
        match bearerToken.Length with
        | 0 -> raise InvalidTokenException
        | _ -> new ClaimsPrincipal(new ClaimsIdentity([new Claim("userId", "42")]))
    
    let getApiVersion () =
        "1.0.0"
                                    
    let app = functionApp {
        outputSourcePath "/Users/jamesrandall/code/authoredSource"
        defaultAuthorizationMode Token
        tokenValidator validateToken
        claimsMappings [
            claimsMapper.shared ("userId", "userId")
            //claimsMapper.command ("userId", (fun cmd -> cmd.userId) )
        ]
        httpRoute "version" [
            azureFunction.http (getApiVersion, Get)
        ]
    }
                
