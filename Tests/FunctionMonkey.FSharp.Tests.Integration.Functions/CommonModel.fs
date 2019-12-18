module FunctionMonkey.FSharp.Tests.Integration.Functions.CommonModel

exception ShouldNotBeCalledException

type SimpleResponse =
    {
        value: int
        message: string
    }
    
let successfulSimpleResponse =
    {
        value = 1
        message = "success"
    }
    
let successfulSimpleResponseCollection =
    [
        { value = 1 ; message = "success1" }
        { value = 2 ; message = "success2" }
    ]

type SeverityEnum =
    | Error = 0
    | Warning = 1
    | Info = 2

type ValidationError =
    {
        severity: SeverityEnum
        errorCode: string
        property: string
        message: string
    }
