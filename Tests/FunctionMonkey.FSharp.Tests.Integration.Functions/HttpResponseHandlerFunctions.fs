module FunctionMonkey.FSharp.Tests.Integration.Functions.HttpResponseHandlerFunctions
open FunctionMonkey.Commanding.Abstractions.Validation
open FunctionMonkey.FSharp.Models
open FunctionMonkey.FSharp.Configuration
open FunctionMonkey.FSharp.Tests.Integration.Functions.CommonModel
open Microsoft.AspNetCore.Mvc

// Function monkey currently uses type names to generate the names of the compiled functions hence this duplication
// A planned improvement is to allow names to be specified and types to be reused.
type HttpResponseHandlerCommandWithNoResultAndNoValidation =
    {
        value: int
    }
type HttpResponseHandlerCommandWithNoResultAndValidatorThatFails =
    {
        value: int
    }
type HttpResponseHandlerCommandWithNoResultAndValidatorThatPasses =
    {
        value: int
    }
type HttpResponseHandlerCommandWithResultAndNoValidation =
    {
        value: int
    }
type HttpResponseHandlerCommandWithResultAndValidatorThatFails =
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
    
    
let private noResultAndNoValidation (_ : HttpResponseHandlerCommandWithNoResultAndNoValidation) =
    System.Console.WriteLine("noResultAndNoValidation executed")
    ()

let failingValidator _ = validationFailure ()
let succeedingValidator _ = validationSuccess ()

let private noResultAndValidatorThatFails (_ : HttpResponseHandlerCommandWithNoResultAndValidatorThatFails) : unit =
    raise ShouldNotBeCalledException
    
let private noResultAndValidatorThatPasses (_ : HttpResponseHandlerCommandWithNoResultAndValidatorThatPasses) =
    ()
    
let private resultAndNoValidation (_ : HttpResponseHandlerCommandWithResultAndNoValidation) =
    successfulSimpleResponse
    
let private resultAndValidatorThatFails (_ : HttpResponseHandlerCommandWithResultAndValidatorThatFails) =
    raise ShouldNotBeCalledException

let resultAndValidatorThatPasses (_ : HttpResponseHandlerCommandWithResultAndValidatorThatPasses) =
    successfulSimpleResponse

let responseHandler _ result = async {
    let resultObj = result :> obj
    return match resultObj with
           | :? unit ->  OkObjectResult("CreateResponse<TCommand>") :> IActionResult
           | _ -> OkObjectResult("CreateResponse<TCommand,TResult>") :> IActionResult
}

let exceptionResponseHandler _ _ = async {
    return OkObjectResult("CreateResponseFromException<TCommand>") :> IActionResult
}

let validationFailureResponseHandler _ _ = async {
    return OkObjectResult("CreateValidationFailureResponse<TCommand>") :> IActionResult
}

let httpNoResponseHandlerFunctions = functions {
    httpRoute "responseHandler" [
        azureFunction.http (
            Handler(noResultAndNoValidation),
            Get,
            subRoute="/noResult/noValidation",
            asyncResponseHandler=responseHandler,
            asyncExceptionResponseHandler=exceptionResponseHandler,
            asyncValidationFailureResponseHandler=validationFailureResponseHandler
        )
        azureFunction.http (
            Handler(noResultAndValidatorThatFails),
            Get,
            subRoute="/noResult/validationFails",
            validator=failingValidator,
            asyncResponseHandler=responseHandler,
            asyncExceptionResponseHandler=exceptionResponseHandler,
            asyncValidationFailureResponseHandler=validationFailureResponseHandler
        )
        azureFunction.http (
            Handler(noResultAndValidatorThatPasses),
            Get,
            subRoute="/noResult/validationPasses",
            validator=succeedingValidator,
            asyncResponseHandler=responseHandler,
            asyncExceptionResponseHandler=exceptionResponseHandler,
            asyncValidationFailureResponseHandler=validationFailureResponseHandler
        )
        azureFunction.http (
            Handler(resultAndNoValidation),
            Get,
            subRoute="/result/noValidation",
            asyncResponseHandler=responseHandler,
            asyncExceptionResponseHandler=exceptionResponseHandler,
            asyncValidationFailureResponseHandler=validationFailureResponseHandler
        )
        azureFunction.http (
            Handler(resultAndValidatorThatFails),
            Get,
            subRoute="/result/validationFails",
            validator=failingValidator,
            asyncResponseHandler=responseHandler,
            asyncExceptionResponseHandler=exceptionResponseHandler,
            asyncValidationFailureResponseHandler=validationFailureResponseHandler
        )
        azureFunction.http (
            Handler(resultAndValidatorThatPasses),
            Get,
            subRoute="/result/validationPasses",
            validator=succeedingValidator,
            asyncResponseHandler=responseHandler,
            asyncExceptionResponseHandler=exceptionResponseHandler,
            asyncValidationFailureResponseHandler=validationFailureResponseHandler
        )
    ]
}

