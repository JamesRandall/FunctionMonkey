# Introduction

AzureFromTheTrenches.Commanding is a configuration based asynchronous command mediator framework with a number of key design goals:

* To provide a highly performant mediator for simple usage
* To support evolution across a projects lifecycle allowing for easy decomposition from a modular-monolith to a fully distributed micro-service architecture
* To provide a none-leaking abstraction over command dispatch and execution semantics
* To reduce boilerplate code - simplistically less code means less errors and less to maintain

To support these goals the framework supports .NET Standard 2.0 (and higher) and so can be used in a wide variety of scenarios and a number of fully optional extension packages are available to enable:

* Building a [REST API](restApi/quickstart.md) directly from commands using a configuration based approach
* Dispatching commands to queues (Service Bus Queues and Topics, and Azure Storage)
* Dispatching commands to event hubs
* Using queues as a source for executing commands 
* Caching commands based on signatures in local memory caches or Redis caches

You don't need to take advantage of that functionality but you can, if you want, adopt it over time without changing your core code.

For an introduction on moving from a layered "onion" architecture to a mediated command approach that makes use of  this framework [please see this series of posts here](https://www.azurefromthetrenches.com/c-cloud-application-architecture-commanding-with-a-mediator-the-full-series/).
