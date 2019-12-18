module FunctionMonkey.FSharp.Tests.Integration.Functions.HttpWithLoggerFunctions
open FunctionMonkey.FSharp.Models
open FunctionMonkey.FSharp.Configuration

type HttpGetWithLoggerCommand =
    {
        value: int
        message: string
    }
    
    
let private withLogger (_ : HttpGetWithLoggerCommand) =
    // In F# land this test isn't necessary but we just do a minimal implementation so we can use the same suite of
    // integration test functions as in C#
    System.Console.WriteLine("withLogger executed")
    ()

let httpWithLoggerFunctions = functions {
    httpRoute "withLogger" [
        azureFunction.http (Handler(withLogger), Get)
    ]
}
