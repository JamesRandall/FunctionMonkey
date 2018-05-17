# Function Monkey

Write more elegant Azure Functions with less boilerplate, more consistency, and support for REST APIs.

_Please note that this project is still under development - packages are available but in a beta state_

By being opinionated about a core pattern (mediation) this framework aims to simplify the writing of Azure Functions removing the boilerplate and providing a framework for common cross cutting concerns (as you find when you move beyond basic functions serverless is not serverless and you _do_ need to think about shared state and configuration for performant functions).

As a by-product it makes it easy to write REST APIs with nice routes, model validation, token based security (e.g. OpenID Connect), and OpenAPI / Swagger support.

Documentation will follow but for now an [introduction can be found on my blog](https://www.azurefromthetrenches.com/build-elegant-rest-apis-with-azure-functions/).
