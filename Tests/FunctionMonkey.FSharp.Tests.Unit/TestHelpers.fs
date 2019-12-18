module FunctionMonkey.FSharp.Tests.Unit.TestHelpers

type SimpleCommand = {
    value: int
    message: string
}

type SimpleResponse = {
    doubledValue: int
    echoedMessage: string
}

let simpleCommandHandler command =
    {
        doubledValue = command.value * 2
        echoedMessage = command.message
    }