module FunctionMonkey.Tests.FSharp.Integration.Functions.HttpNoResponseHandlerFunctions
open FunctionMonkey.Commanding.Abstractions.Validation
open FunctionMonkey.FSharp.Models
open FunctionMonkey.FSharp.Configuration
open FunctionMonkey.Tests.FSharp.Integration.Functions.CommonModel

// Function monkey currently uses type names to generate the names of the compiled functions hence this duplication
// A planned improvement is to allow names to be specified and types to be reused.
type HttpCommandWithNoResultAndNoValidation =
    {
        value: int
    }
type HttpCommandWithNoResultAndValidatorThatFails =
    {
        value: int
    }
type HttpCommandWithNoResultAndValidatorThatPasses =
    {
        value: int
    }
type HttpResponseHandlerCommandWithResultAndNoValidation =
    {
        value: int
    }
type HttpCommandWithResultAndValidatorThatFails =
    {
        value: int
    }
type HttpResponseHandlerCommandWithResultAndValidatorThatPasses =
    {
        value: int
    }
    
let private validationFailure () =
    ValidationResult(Errors=[
        FunctionMonkey.Commanding.Abstractions.Validation.ValidationError(
                           Property="Value",
                           ErrorCode="NotEqualValidator",
                           Message="'Value' must not be equal to '0'."
                       )
    ])
    
let private validationSuccess () =
    ValidationResult()
    
    
let private noResultAndNoValidation (_ : HttpCommandWithNoResultAndNoValidation) =
    System.Console.WriteLine("noResultAndNoValidation executed")
    ()

let failingValidator _ = validationFailure ()
let succeedingValidator _ = validationSuccess ()

let private noResultAndValidatorThatFails (_ : HttpCommandWithNoResultAndValidatorThatFails) : unit =
    raise ShouldNotBeCalledException
    
let private noResultAndValidatorThatPasses (_ : HttpCommandWithNoResultAndValidatorThatPasses) =
    ()
    
let private resultAndNoValidation (_ : HttpResponseHandlerCommandWithResultAndNoValidation) =
    successfulSimpleResponse
    
let private resultAndValidatorThatFails (_ : HttpCommandWithResultAndValidatorThatFails) =
    raise ShouldNotBeCalledException

let resultAndValidatorThatPasses (_ : HttpResponseHandlerCommandWithResultAndValidatorThatPasses) =
    successfulSimpleResponse

let httpNoResponseHandlerFunctions = functions {
    httpRoute "noResponseHandler" [
        azureFunction.http (Handler(noResultAndNoValidation), Get, subRoute="/noResult/noValidation")
        azureFunction.http (Handler(noResultAndValidatorThatFails), Get, subRoute="/noResult/validationFails", validator=failingValidator)
        azureFunction.http (Handler(noResultAndValidatorThatPasses), Get, subRoute="/noResult/validationPasses", validator=succeedingValidator)
        azureFunction.http (Handler(resultAndNoValidation), Get, subRoute="/result/noValidation")
        azureFunction.http (Handler(resultAndValidatorThatFails), Get, subRoute="/result/validationFails", validator=failingValidator)
        azureFunction.http (Handler(resultAndValidatorThatPasses), Get, subRoute="/result/validationPasses", validator=succeedingValidator)
    ]
}

