namespace FmFsharpDemo
open FunctionMonkey.FSharp.Models
open FunctionMonkey.FSharp.Configuration

module EntryPoint =
    type Order = {
        id: string
        customer: string
        value: float
    }
    
    type GetOrderQuery = {
        id: string
    }
    
    type CreateOrderCommand = {
        userId: string
        order: Order
    }
    
    let getOrderQuery query =
        {
            id = query.id
            customer = "Fred Smith"
            value = 95.
        }
        
    let createOrderCommand command =
        printf "Creating order for user %s" command.userId
        
    let validateTokenAsync (bearerToken:string) =
        async {
            return bearerToken.Length > 0
        } // |> Async.StartAsTask // - for ref for tomorrow
                                    
    let app = functionApp {
        tokenValidatorAsync validateTokenAsync
        httpRoute "api/v1/order" [
            azureFunction.http (getOrderQuery, Get, "/{id}")
            azureFunction.http (createOrderCommand, Post)
        ]
    }
                
