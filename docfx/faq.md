# Frequently Asked Questions

## Does this work on Azure Functions v1?

No and it likely never will. Its technically possible but v2 is the future and contains many, many, improvements.

## When will it come out of preview?

Aim is to move it out of preview when Azure Functions v2 go to GA. Certainly won't be before as until then v2 is subject to breaking change (though I think they've done a good job to date in terms of managing breaking change).

## I get the error "Error processing configuration of Function Proxies"

To provide a clean and shaped REST API Function Monkey creates an [Azure Function Proxy](https://docs.microsoft.com/en-us/azure/azure-functions/functions-proxies) for each HTTP function. To support running locally and in the cloud the proxies are created using a variable to specify the domain name of the underlying function. This variable is called _ProxyPrefix_ and requires setting in local.settings.json (for local development) *and* as an Application Setting on the function app in Azure. Locally, unless you configure the function host differently, this should be set to _http://localhost:7071_. For example based on the standard local.settings.json file you should have settings that look like this:

    {
      "IsEncrypted": false,
      "Values": {
        "AzureWebJobsStorage": "UseDevelopmentStorage=true",
        "AzureWebJobsDashboard": "UseDevelopmentStorage=true",
        "FUNCTIONS_WORKER_RUNTIME": "dotnet",
        "ProxyPrefix":  "http://localhost:7071" 
      }
    }

## Are Pull Requests welcome?

Hell yes. But if you're planning a major piece of work probably makes sense to have a conversation first. I'd hate to have folk waste there time.

## How do I get help?

If you do encounter any issues or have any feedback please do let me know in the [GitHub issue tracker](https://github.com/JamesRandall/FunctionMonkey/issues) or on [Twitter](https://twitter.com/azuretrenches) - I might not get around to it immediately but I really appreciate it!

_Please note that Function Monkey, like Azure Functions v2, is still in preview and so is subject to breaking change. That said the public API for Function Monkey has settled down nicely at this point and is being used in a number of projects._



