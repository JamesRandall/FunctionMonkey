module FunctionMonkey.FSharp.Tests.Integration.Functions.HttpClaimFunctions
open FunctionMonkey.FSharp.Models
open FunctionMonkey.FSharp.Configuration
open FunctionMonkey.FSharp.Tests.Integration.Functions.CommonModel

type HttpIntClaimCommand =
    {
        mappedValue: int
    }
    
type HttpStringClaimCommand =
    {
        stringClaim: string
    }
    

    
let private intClaimRespond (command : HttpIntClaimCommand) : SimpleResponse =
    {
        value = command.mappedValue
        message = ""
    }
    
let private stringClaimRespond (command : HttpStringClaimCommand) : SimpleResponse =
    {
        value = 0
        message = command.stringClaim
    }
    
let httpVerbFunctions = functions {
    httpRoute "claims" [
        azureFunction.http (Handler(intClaimRespond), Get, subRoute = "/int", authorizationMode=AuthorizationMode.Token)
        azureFunction.http (Handler(stringClaimRespond), Get, subRoute="/string", authorizationMode=AuthorizationMode.Token)
    ]
}

