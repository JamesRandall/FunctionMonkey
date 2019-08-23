namespace FmFsharpDemo
open AccidentalFish.FSharp.Validation
open System.Security.Claims
open FunctionMonkey.FSharp.Configuration
open FunctionMonkey.FSharp.Models

module EntryPoint =
    exception InvalidTokenException
    
    let validateToken (bearerToken:string) =
        match bearerToken.Length with
        | 0 -> raise InvalidTokenException
        | _ -> new ClaimsPrincipal(new ClaimsIdentity([new Claim("userId", "2FF4D861-F9E3-4694-9553-C49A94D7E665")]))
    
    let getApiVersion () =
        "1.0.0"
        
    let isResultValid (result:ValidationState) =
        match result with
        | Ok -> true
        | _ -> false
                                    
    let app = functionApp {
        // authorization
        defaultAuthorizationMode Token
        tokenValidator validateToken
        claimsMappings [
            claimsMapper.shared ("userId", "userId")
        ]
        // validation
        isValid isResultValid
        // functions
        httpRoute "version" [
            azureFunction.http (Handler(getApiVersion), Get)
        ]
    }
                
