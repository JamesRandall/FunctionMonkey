# Function Monkey

Write more elegant Azure Functions with less boilerplate, more consistency, and support for REST APIs.

_Please note that this project is still under development - packages are available but in a beta state_

_See the note on proxies below_

By being opinionated about a core pattern (mediation) this framework aims to simplify the writing of Azure Functions removing the boilerplate and providing a framework for common cross cutting concerns (as you find when you move beyond basic functions serverless is not serverless and you _do_ need to think about shared state and configuration for performant functions).

As a by-product it makes it easy to write REST APIs with nice routes, model validation, token based security (e.g. OpenID Connect), and OpenAPI / Swagger support.

Documentation will follow but for now an [introduction can be found on my blog](https://www.azurefromthetrenches.com/build-elegant-rest-apis-with-azure-functions/).

## Proxies

The recent update for the Functions CLI tools contained a bug that caused function projects with proxies.json files to crash on startup - you'll know if it happens as you'll get a screen full of red text in the console. This is only a problem locally - they are fine in Azure itself.

This has been fixed in the latest version but the Visual Studio Tools don't yet reference it.

To workaround this you can either disable proxies:

    .ProxiesEnabled(false)

Or update the CLI tools to the latest version and ensure Visual Studio uses them. In the below I'm assuming you installed the tools through npm (at the time of writing the choco package isn't yet updated... which would kind of suggest you'd be better sticking with npm until cadences are matched):

    npm i -g azure-functions-core-tools@core --unsafe-perm true

Then in Visual Studio:

1. Open the project properties for your Functions project
2. Go to the Debug tab
3. In the Additional Arguments section paste the below (correcting for your username):

    C:\Users\<username>\AppData\Roaming\npm\node_modules\azure-functions-core-tools\bin\func.dll host start

If your global npm packages are elsewhere then you'll need to correct the path.