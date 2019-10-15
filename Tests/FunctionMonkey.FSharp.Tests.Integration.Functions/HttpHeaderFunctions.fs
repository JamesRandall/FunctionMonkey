module FunctionMonkey.FSharp.Tests.Integration.Functions.HttpHeaderFunctions
open System
open FunctionMonkey.FSharp.Models
open FunctionMonkey.FSharp.Configuration
open FunctionMonkey.FSharp.Tests.Integration.Functions.CommonModel

type HttpDefaultHeaderCommand =
    {
        defaultHeaderIntValue: int
        defaultHeaderStringValue: string
    }
type HttpHeaderBindingCommand =
    {
        value: int
        message: string
    }
    
type HttpHeaderNullableValueTypeBindingCommand =
    {
        value: int option
    }
    
type BindingTestEnum =
    | SomeValue = 56
    | AnotherValue = 92316
    
type HttpHeaderEnumTypeBindingCommand =
    {
        value: BindingTestEnum
    }

let private headerBinding (command : HttpHeaderBindingCommand) : SimpleResponse =
    {
        value = command.value
        message = command.message
    }
    
let private headerNullableValueTypeBinding (command : HttpHeaderNullableValueTypeBindingCommand) : SimpleResponse =
    {
        value = (match command.value with | Some v -> v | None -> Int32.MinValue)
        message = ""
    }

let private headerEnumTypeBinding (command : HttpHeaderEnumTypeBindingCommand) : SimpleResponse =
    {
        value = int(command.value)
        message = ""
    }
    
let private defaultHeaderCommand (command : HttpDefaultHeaderCommand) : SimpleResponse =
    {
        value = command.defaultHeaderIntValue
        message = command.defaultHeaderStringValue
    }

let httpWithLoggerFunctions = functions {
    httpRoute "headers" [
        azureFunction.http (Handler(headerBinding), Get, headerMappings =
            [
                header.mapping((fun cmd -> cmd.value), "x-value")
                header.mapping((fun cmd -> cmd.message), "x-message")
            ]
        )
        azureFunction.http (
            Handler(headerNullableValueTypeBinding),
            Get,
            subRoute="/nullableValueType",
            headerMappings =
                [
                    header.mapping((fun cmd -> cmd.value), "x-value")
                ]
        )
        azureFunction.http (
            Handler(headerEnumTypeBinding),
            Get,
            subRoute="/enumType",
            headerMappings =
                [
                    header.mapping((fun cmd -> cmd.value), "x-enum-value")
                ]
        )
        azureFunction.http (
            Handler(defaultHeaderCommand),
            Get,
            subRoute="/default",
            headerMappings =
                [
                    // TODO: When we add default header mappings in we need to take these out
                    // and move to the entry point
                    header.mapping((fun cmd -> cmd.defaultHeaderIntValue), "x-default-int")
                    header.mapping((fun cmd -> cmd.defaultHeaderStringValue), "x-default-string")
                ]
        )
    ]
}
