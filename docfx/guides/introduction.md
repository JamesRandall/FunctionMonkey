# Introduction

Function Monkey is an alternative API for creating Azure Functions which by having a strong opinion over a core pattern, mediation, is able to achieve a number of key design goals:

* To allow functions to be created and configured with a clean fluent style API
* To reduce the typical boilerplate associated with functions
* To separate the implementation of a function from its trigger and wrap around infrastructural code
* To provide a minimal runtime framework for typical C# / .NET patterns such as IoC and input validation
* To enable REST APIs to be elegantly built along with support for bearer token authorization, claims, and Swagger/OpenApi
* To improve the testability of functions
* To co-exist in a project with standard attribute based Azure Functions

Function Monkey is built on [Azure Functions v2](https://docs.microsoft.com/en-us/azure/azure-functions/functions-overview) and the [AzureFromTheTrenches.Commanding](https://commanding.azurefromthetrenches.com) mediation framework and provided as a .NET Standard 2.0 package.

_Please note that Function Monkey, like Azure Functions v2, is still in preview and so is subject to breaking change. That said the public API for Function Monkey has settled down nicely at this point and is being used in a number of projects._

_If you do encounter any issues or have any feedback please do let me know in the [GitHub issue tracker](https://github.com/JamesRandall/FunctionMonkey/issues) or on [Twitter](https://twitter.com/azuretrenches) - I might not get around to it immediately but I really appreciate it!_

