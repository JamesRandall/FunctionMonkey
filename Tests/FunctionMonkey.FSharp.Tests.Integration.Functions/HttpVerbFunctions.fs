module FunctionMonkey.FSharp.Tests.Integration.Functions.HttpVerbFunctions
open FunctionMonkey.FSharp.Models
open FunctionMonkey.FSharp.Configuration
open FunctionMonkey.FSharp.Tests.Integration.Functions.CommonModel

type HttpGetCommand =
    {
        value: int
        message: string
    }
    
type HttpPostCommand =
    {
        value: int
        message: string
    }
    
type HttpPutCommand =
    {
        value: int
        message: string
    }
    
type HttpDeleteCommand =
    {
        value: int
        message: string
    }
    
type HttpPatchCommand =
    {
        value: int
        message: string
    }
    
type HttpPostWithBytesCommand =
    {
        bytes: byte array
    }
    
let private getRespond (command : HttpGetCommand) : SimpleResponse =
    {
        value = command.value
        message = command.message
    }
    
let private postRespond (command : HttpPostCommand) : SimpleResponse =
    {
        value = command.value
        message = command.message
    }
    
let private putRespond (command : HttpPutCommand) : SimpleResponse =
    {
        value = command.value
        message = command.message
    }
    
let private deleteRespond (command : HttpDeleteCommand) : SimpleResponse =
    {
        value = command.value
        message = command.message
    }
    
let private patchRespond (command : HttpPatchCommand) : SimpleResponse =
    {
        value = command.value
        message = command.message
    }
    
let private postWithBytesRespond (command : HttpPostWithBytesCommand) =
    command
    
let private getWithNoParameters () : SimpleResponse =
    {
        value = 42
        message = "Some text"
    }

let httpVerbFunctions = functions {
    httpRoute "verbs" [
        azureFunction.http (Handler(getWithNoParameters), Get)
        azureFunction.http (Handler(getRespond), Get, subRoute = "/{value}")
        azureFunction.http (Handler(postRespond), Post)
        azureFunction.http (Handler(postWithBytesRespond), Post, subRoute = "/bytes")
        azureFunction.http (Handler(putRespond), Put)
        azureFunction.http (Handler(deleteRespond), Delete, subRoute = "/{value}")
        azureFunction.http (Handler(patchRespond), Patch)
    ]
}

