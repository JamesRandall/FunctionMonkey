---
title: "Quickstart"
date: 2020-02-08T21:30:00+01:00
draft: false
weight: 3
---

To demonstrate how to use Function Monkey we'll build a super simple ToDo application. The full source for this, along with a demonstration of how to validate inputs, [can be found on GitHub](https://github.com/JamesRandall/FunctionMonkey-ToDo-FSharp).

First create an empty Azure Functions project targetting .NET Core 3.1 and, of course, using F#.

Next add the following NuGet packages:

    FunctionMonkey.FSharp
    FunctionMonkey.Compiler

_Note that at the time of writing you will need to use prerelease packages version 4.0.43-beta.4 or later._

First create a file called ToDoItem.fs and create a record type for our ToDo items:

{{< highlight fsharp >}}
type ToDoItem = {
    id: System.Guid
    title: string
    isComplete: bool
}
{{< / highlight >}}

Next lets create a really simple repositoy for ToDo items based around an array. Create a file called MemoryRepository.fs and add functions that let us add, get and mark items as complete:

{{< highlight fsharp >}}
let mutable private items : ToDoItem[] = [||]

let add title =
    let newItem = {
        title = title
        id = System.Guid.NewGuid()
        isComplete = false
    }
    items <- Array.append items [|newItem|]
    newItem
    
let getAll () = items

let markComplete id =
    items <-
        items
        |> Array.map (fun i -> match i.id = id with
                               | true -> { i with isComplete = true }
                               | false -> i)
{{< / highlight >}}

So far so good. Now lets wire this up to a REST API. Function Monkey uses a DSL to associate functions with triggers. When a trigger is called Function Monkey deserializes the input data, annotates it as appropriate (for example mapping access token claims to fields for an HTTP trigger) and then calling the handler function. If you're not interested in the payload from the trigger then you can skip defining a record and just hand a parameterless function to Function Monkey. There's more to the DSL than that (piping to output triggers, authentication / authorization support, and much more) but this is all we need to get going.

We'll use record types for creating an item and marking an item as complete and a parameterless function for getting all the todo items.

Begin by creating a file called FunctionApp.fs and creating two record types:

{{< highlight fsharp >}}
type AddToDoItemCommand = {
    title: string
}

type MarkAsCompleteCommand = {
    id: Guid
}
{{< / highlight >}}

Now we'll create two functions for handling triggers associated with these commands (we don't need one for getting all the items as we'll see):

{{< highlight fsharp >}}
let addToDoItem addToRepository command =
    addToRepository command.title
    
let markAsComplete markAsCompleteInRepository command =
    markAsCompleteInRepository command.id
{{< / highlight >}}

These two functions accept repository update functions as their first parameter and their respective command as the second parameter. Strictly speaking in this simple example we don't need to do this but its worth demonstrating how you can use currying with Function Monkey to inject dependencies.

Finally we declare a function application using Function Monkey's DSL:

{{< highlight fsharp >}}
let app = functionApp {
    httpRoute "todo" [
        azureFunction.http (Handler(addToDoItem MemoryRepository.add), Post)
        azureFunction.http (Handler(MemoryRepository.getAll), Get)
        azureFunction.http (Handler(markAsComplete MemoryRepository.markComplete), Put)
    ]
}
{{< / highlight >}}

In this block we declare a route called todo and associate three HTTP triggers with it on different verbs. We curry our addToDoItem and markAsComplete functions with our reporistory functions and for the get verb we simply use the repository get all method directly.

There's one last thing to do before running this. We're going to remove the "api" prefix that by default Azure Functions applies to the route of every HTTP function. Edit your host.json file (add one if you don't have one):

{{< highlight json >}}
{
    "version": "2.0",
    "extensions": {
        "http": {
            "routePrefix": ""
        }
    }
}
{{< / highlight >}}

With that complete you should be able to run 