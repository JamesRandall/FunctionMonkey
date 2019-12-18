module FunctionMonkey.FSharp.Tests.Integration.Functions.HttpRouteParameterFunctions
open System
open FunctionMonkey.FSharp.Models
open FunctionMonkey.FSharp.Configuration

type HttpGetRouteParameterCommandAndResponse =
    {
        optionalValue: int option
        value: int
        message: string
        optionalMessage: string option
    }
    
type HttpGetGuidRouteParameterCommand =
    {
        requiredGuid: Guid
        optionalGuid: Guid option
    }
    
type GuidPairResponse =
    {
        valueOne: Guid
        valueTwo: Guid option
    }
    
let private getRouteParameter (command : HttpGetRouteParameterCommandAndResponse) : HttpGetRouteParameterCommandAndResponse =
    command

let private getGuidRouteParameter (command : HttpGetGuidRouteParameterCommand) : GuidPairResponse =
    {
        valueOne = command.requiredGuid
        valueTwo = command.optionalGuid
    }

let httpVerbFunctions = functions {
    httpRoute "routeParameters" [
        azureFunction.http (Handler(getRouteParameter), Get, subRoute = "/{message}/{value:int}/{optionalValue?}/{optionalMessage?}")
        azureFunction.http (Handler(getGuidRouteParameter), Get, subRoute = "/guids/{requiredGuid}/{optionalGuid?}")
    ]
}
