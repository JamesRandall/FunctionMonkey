namespace FmFsharpDemo
open AccidentalFish.FSharp.Validation
open AzureFromTheTrenches.Commanding.Abstractions
open System
open FunctionMonkey.FSharp.Models
open FunctionMonkey.FSharp.Configuration
open FunctionMonkey.FSharp.OutputBindings
open CosmosDb
open EntryPoint

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
            [<SecurityProperty>]
            userId: string
            title: string
            isComplete: bool
        }
        
    type UpdateToDoItemCommand =
        {
            [<SecurityProperty>]
            userId: string
            id: string
            title: string
            isComplete: bool
        }
        
    type GetToDoItemQuery =
        {
            [<SecurityProperty>]
            userId: string
            id: string
        }
        
    type GetToDoItemParamQuery =
        {
            [<SecurityProperty>]
            userId: string
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
    
    let validateUpdateToDoItemCommand = createValidatorFor<UpdateToDoItemCommand>() {
        validate (fun q -> q.id) withIdValidations
        validate (fun c -> c.userId) withIdValidations
        validate (fun c -> c.title) withTitleValidations
    }
    
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
    
    let addToDoItem (command:AddToDoItemCommand) =
        {
            id = Guid.NewGuid().ToString()
            owningUserId = command.userId
            title = command.title
            isComplete = command.isComplete
        }
        
    let getToDoItem (query:GetToDoItemQuery) =
        async {
            let! result = CosmosDb.reader<ToDoItem> <| query.id
            if result.owningUserId = query.userId then raise AuthorizationException
            return result
        }
        
    let getToDoItemParam (query:GetToDoItemParamQuery) =
        async {
            let! result = CosmosDb.reader<ToDoItem> <| query.id
            if result.owningUserId = query.userId then raise AuthorizationException
            return result
        }
        
    let updateToDoItem (updateToDoItemCommand:UpdateToDoItemCommand) =
        async {
            let! existingItem = CosmosDb.reader<ToDoItem> <| updateToDoItemCommand.id
            if not (existingItem.owningUserId = updateToDoItemCommand.userId) then
                raise AuthorizationException
            return { existingItem with title = updateToDoItemCommand.title
                                       isComplete = updateToDoItemCommand.isComplete }  
        }        
    
    let todoDatabase =
        cosmosDb cosmosCollection cosmosDatabase
           
    let toDoFunctions = functions {
        httpRoute "api/v1/todo" [
            azureFunction.http (AsyncHandler(getToDoItemParam),
                                verb=Get, subRoute="/withParam")
            azureFunction.http (AsyncHandler(getToDoItem),
                                verb=Get, subRoute="/{id}")
                                //validator=validateGetToDoItemQuery)
            azureFunction.http (Handler(addToDoItem),
                                verb=Post,
                                validator=validateAddToDoItemCommand,
                                returnResponseBodyWithOutputBinding=true)
                |> todoDatabase
            azureFunction.http (AsyncHandler(updateToDoItem), verb=Put, validator=validateUpdateToDoItemCommand)
                |> todoDatabase
        ]
    }
    
