module FunctionMonkey.FSharp.Tests.Unit.EntryPoint

open Expecto

[<EntryPoint>]
let main args =
    runTestsInAssemblyWithCLIArgs [||] args
    