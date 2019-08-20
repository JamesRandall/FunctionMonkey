namespace FmFsharpDemo
open AccidentalFish.FSharp.Validation
open System
open FunctionMonkey.FSharp.Models
open FunctionMonkey.FSharp.Configuration
open FunctionMonkey.FSharp.Models
open Result

module ToDo =
    type ToDoItem =
        {
            id: string
            owningUserId: string
            title: string
            complete: bool
        }
        
    type AddToDoItemCommand =
        {
            userId: string
            title: string
            complete: bool
        }
        
    let validateAddToDoItemCommand = createValidatorFor<AddToDoItemCommand>() {
        validate (fun c -> c.userId) [
            isNotEmpty
            hasLengthOf 36
        ]
        validate (fun c -> c.title) [
            isNotEmpty
            hasMinLengthOf 1
            hasMaxLengthOf 255
        ]
    }
        
    let addToDoItem command =
        async {
            let newItem = {
                id = Guid.NewGuid().ToString()
                owningUserId = command.userId
                title = command.title
                complete = command.complete
            }
            
            return newItem.id
        }
                
    let toDoFunctions = functions {
        httpRoute "api/v1/todo" [
            azureFunction.http (addToDoItem, Post, validator=validateAddToDoItemCommand)
        ]
    } 

