namespace FmFsharpDemo
open AccidentalFish.FSharp.Validation
open System
open FunctionMonkey.FSharp.Models
open FunctionMonkey.FSharp.Configuration
open FunctionMonkey.FSharp.OutputBindings
open CosmosDb

module ToDo =
    type ToDoItem =
        {
            id: string
            owningUserId: string
            title: string
            isComplete: bool
        }
        
    type AddToDoItemCommand =
        {
            userId: string
            title: string
            isComplete: bool
        }
        
    type GetToDoItemQuery =
        {
            id: string
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
    
    let validateGetToDoItemQuery = createValidatorFor<GetToDoItemQuery>() {
        validate (fun q -> q.id) withIdValidations
    }
        
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
        {
            id = Guid.NewGuid().ToString()
            owningUserId = command.userId
            title = command.title
            isComplete = command.isComplete
        }
        
    let getToDoItem query =
        CosmosDb.reader<ToDoItem> <| query.id
        
    let todoDatabase =
        cosmosDb cosmosCollection cosmosDatabase
                
    let toDoFunctions = functions {
        httpRoute "api/v1/todo" [
            azureFunction.http (AsyncHandler(getToDoItem),
                                verb=Get, subRoute="/{id}",
                                validator=validateGetToDoItemQuery)
            azureFunction.http (Handler(addToDoItem),
                                verb=Post,
                                validator=validateAddToDoItemCommand,
                                returnResponseBodyWithOutputBinding=true)
                |> todoDatabase
            azureFunction.http (NoHandler, verb=Put, validator=validateToDoItem)
                |> todoDatabase
        ]
    }


