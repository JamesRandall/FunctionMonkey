module FunctionMonkey.FSharp.Tests.Integration.Functions.HttpSecurityPropertyFunctions
open AzureFromTheTrenches.Commanding.Abstractions
open AzureFromTheTrenches.Commanding.Abstractions
open FunctionMonkey.FSharp.Models
open FunctionMonkey.FSharp.Configuration
open FunctionMonkey.FSharp.Tests.Integration.Functions.CommonModel
open Newtonsoft.Json

type HttpGetCommandWithSecurityProperty =
    {
        [<SecurityProperty>]
        value: int
        message: string
    }
    
type HttpPostCommandWithSecurityProperty =
    {
        [<SecurityProperty>]
        value: int
        message: string
    }
    
let private getRespond (command : HttpGetCommandWithSecurityProperty) : SimpleResponse =
    {
        value = command.value
        message = command.message
    }
    
let private postRespond (command : HttpPostCommandWithSecurityProperty) : SimpleResponse =
    {
        value = command.value
        message = command.message
    }

let httpVerbFunctions = functions {
    httpRoute "securityProperty" [
        azureFunction.http (Handler(getRespond), Get)
        azureFunction.http (Handler(postRespond), Post)
    ]
}

