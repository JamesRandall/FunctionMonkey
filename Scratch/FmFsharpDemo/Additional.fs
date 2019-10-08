module FmFsharpDemo.Additional
open FunctionMonkey.FSharp.OutputBindings
open FunctionMonkey.FSharp.Configuration
open FunctionMonkey.FSharp.Models

type SomeCommand =
    {
        message: string
    }

let queueHandler c =
    System.Console.WriteLine("SbQueueCommand: " + c.message)
    [50]
(*
let additionalFunctions = functions {
    serviceBus DefaultConnectionStringSettingName [
        azureFunction.serviceBusQueue (Handler(queueHandler), "sbQueueCommand")
            |> serviceBusQueue ("junk")
    ]
}
*)