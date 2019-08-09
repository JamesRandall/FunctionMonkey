namespace FmFsharpDemo
open AzureFromTheTrenches.Commanding.Abstractions
open FunctionMonkey.Abstractions
open FunctionMonkey.Abstractions.Builders
open FunctionMonkey.FSharp.Configuration

module EntryPoint =
    type OrderId =
        | OrderId of string
    type UserId =
        | UserId of string
    
    type Order = {
        id: OrderId
        customer: string
        value: float
    }
    
    type GetOrderQuery = {
        id: OrderId
    }
    
    type CreateOrderCommand = {
        userId: UserId
        order: Order
    }
    
    let getOrderQuery query =
        {
            id = query.id
            customer = "Fred Smith"
            value = 95.
        }
        
    let createOrderCommand newOrder =
        printf "Creating order"
        
    
    let loggingHandler (cmd:'a) (handler:'a -> 'b) =
        printf "Running"
        let result = handler cmd
        printf "Run"
        result
            
    let c = functionApp {
        httpRoute "/api/v1/order" [
            azureFunction.http<GetOrderQuery, Order> (getOrderQuery, Get, "/{id}")
            azureFunction.http<CreateOrderCommand> (createOrderCommand, Post)
        ]
        
        start
    }
            
    let app = functionApp {
        httpRoute "/api/v1/order" [
            azureFunction.http<GetOrderQuery, Order> (getOrderQuery, Get, "/{id}")
            azureFunction.http<CreateOrderCommand> (createOrderCommand, Post)
        ]
        
        start
    }
                
