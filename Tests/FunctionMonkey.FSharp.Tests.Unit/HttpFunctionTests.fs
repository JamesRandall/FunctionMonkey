module FunctionMonkey.FSharp.Tests.Unit.HttpFunctionTests

open Expecto
open FunctionMonkey.FSharp.Configuration
open FunctionMonkey.FSharp.Models
open FunctionMonkey.FSharp.Tests.Unit
open FunctionMonkey.Model
open FunctionMonkey.Model
open System.Linq
open TestHelpers

[<Tests>]
let routeParameterExtractionTests =
    testList "Parameter extractions" [
        test "Extracts constructor parameters for record based command" {
            let app = functionApp {
                disableFunctionModules
                httpRoute "/api/v1/test" [
                    azureFunction.http (Handler(simpleCommandHandler), HttpVerb.Get, "/{value}") 
                ]
            }
            
            Expect.hasLength app.FunctionDefinitions 1 "Should have 1 function definition"
            let httpFunction = app.FunctionDefinitions.Single() :?> HttpFunctionDefinition
            let parameters = httpFunction.ImmutableTypeConstructorParameters
            Expect.hasLength parameters 2 "Should have 2 constructor parameters"
            Expect.equal (parameters.First().Name) "value" "First Parameter should be called value"
            Expect.equal (parameters.First().Type) typeof<int> "First Parameter should be of type int"
            Expect.equal (parameters.Skip(1).First().Name) "message" "First Parameter should be called message"
            Expect.equal (parameters.Skip(1).First().Type) typeof<string> "Second Parameter should be of type string"            
        }
        
        test "Extracts route parameters for record based command" {
            let app = functionApp {
                disableFunctionModules
                httpRoute "/api/v1/test" [
                    azureFunction.http (Handler(simpleCommandHandler), HttpVerb.Get, "/{value}") 
                ]
            }
            
            Expect.hasLength app.FunctionDefinitions 1 "Should have 1 function definition"
            let httpFunction = app.FunctionDefinitions.Single() :?> HttpFunctionDefinition
            let parameters = httpFunction.RouteParameters
            Expect.hasLength parameters 1 "Should have 1 route parameter"
            Expect.equal (parameters.Single().Name) "value" "Route parameter should be called value"
            Expect.equal (parameters.Single().Type) typeof<int> "Route parameter should be of type int"                     
        }
    ]
    