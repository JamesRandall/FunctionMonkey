namespace FmFsharpDemo
open AccidentalFish.FSharp.Validation
open System
open FunctionMonkey.FSharp.Models
open FunctionMonkey.FSharp.Configuration
open FunctionMonkey.FSharp.OutputBindings

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
        
    let withIdValidations = [
       isNotEmpty
       hasLengthOf 36
    ]
    
    let withTitleValidations = [
        isNotEmpty
        hasMinLengthOf 1
        hasMaxLengthOf 255
    ]
        
    let validateAddToDoItemCommand = createValidatorFor<AddToDoItemCommand>() {
        validate (fun c -> c.userId) withIdValidations
        validate (fun c -> c.title) withTitleValidations
    }
    
    let validateToDoItem = createValidatorFor<ToDoItem>() {
        validate (fun c -> c.id) withIdValidations
        validate (fun c -> c.title) withTitleValidations
        validate (fun c -> c.owningUserId) withIdValidations
    }
        
    let addToDoItem command =
        async {
            let newItem = {
                id = Guid.NewGuid().ToString()
                owningUserId = command.userId
                title = command.title
                complete = command.complete
            }
            
            return newItem
        }
                
    let toDoFunctions = functions {
        httpRoute "api/v1/todo" [
            azureFunction.http (AsyncHandler(addToDoItem), verb=Post, validator=validateAddToDoItemCommand)
                |> cosmosDb "dbToDo" "colToDoItems"
            azureFunction.http (NoHandler, verb=Put, validator=validateToDoItem)
                |> cosmosDb "dbToDo" "colToDoItems"
        ]
    } 

