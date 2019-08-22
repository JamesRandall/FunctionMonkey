namespace FmFsharpDemo
open FunctionMonkey.FSharp.Models
open FunctionMonkey.FSharp.Configuration
open Microsoft.AspNetCore.Mvc
open Microsoft.AspNetCore.Mvc

module Orders =
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
        
    let responseHandlerAsync cmd result =
        async {
            return new OkObjectResult("hello world") :> IActionResult
        }
       
    let orderFunctions = functions {
        httpRoute "api/v1/order" [
            azureFunction.http (Handler(getOrderQuery), Get, "/{id}")
            azureFunction.http (Handler(createOrderCommand), Post)
        ]
    }

