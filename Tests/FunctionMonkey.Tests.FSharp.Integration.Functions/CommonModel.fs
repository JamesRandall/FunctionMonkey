module FunctionMonkey.Tests.FSharp.Integration.Functions.CommonModel

type SimpleResponse =
    {
        value: int
        message: string
    }
    
let successfulSimpleResponse =
    {
        value = 1
        message = "Success"
    }
    
let successfulSimpleResponseCollection =
    [
        { value = 1 ; message = "success1" }
        { value = 2 ; message = "success2" }
    ]


