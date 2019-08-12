namespace FmFsharpDemo
open AzureFromTheTrenches.Commanding.Abstractions
open FunctionMonkey.Abstractions
open FunctionMonkey.Abstractions.Builders
open FunctionMonkey.FSharp.Models
open FunctionMonkey.FSharp.Configuration

module EntryPoint =
    type OrderId =
        | OrderId of string
    type UserId =
        | UserId of string
    
    type Order = {
        id: string
        customer: string
        value: float
    }
    
    type GetOrderQuery = {
        id: string // OrderId
    }
    
    type CreateOrderCommand = {
        userId: UserId
        order: Order
    }
    
    let getOrderQuery (query:GetOrderQuery) : Order =
        {
            id = query.id
            customer = "Fred Smith"
            value = 95.
        }
        
    let createOrderCommand command =
        printf "Creating order"
        
    let wrapHandler handler =
        let wrapped cmd =
            printf "before"
            let result = handler cmd
            printf "after"
            result
        wrapped
                                    
    let app = functionApp {
        outputSourcePath "/Users/jamesrandall/code/authoredSource"
        httpRoute "/api/v1/order" [
            azureFunction.http (wrapHandler getOrderQuery, Get, "/{id}")
            //azureFunction.http (wrapHandler createOrderCommand, Post)
        ]
    }
                
