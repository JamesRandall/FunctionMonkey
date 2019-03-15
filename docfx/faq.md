# Frequently Asked Questions

## Does this work on Azure Functions v1?

No and it likely never will. It's technically possible but v2 is the future and contains many, many improvements.

## When will it come out of preview?

Aim is to move it out of preview when Azure Functions v2 go to GA. Certainly won't be before as until then v2 is subject to breaking change (though I think they've done a good job to date in terms of managing breaking change).

## When I open the Open API / Swagger user interface for the first time when hosted in Azure sometimes I get a blank screen, it appears when I refresh my browser.

I'm still investigating this but there looks to be a bug in the cold start behaviour in Azure. The logs show the proxies are not always routed on to Functions and instead the funciton host looks for physical files. I have a GitHub issue raised which hopefully will help me gain more insight.

## Are Pull Requests welcome?

Heck yes. But if you're planning a major piece of work probably makes sense to have a conversation first. I'd hate to have folks waste their time.

## How do I get help?

If you do encounter any issues or have any feedback please do let me know in the [GitHub issue tracker](https://github.com/JamesRandall/FunctionMonkey/issues) or on [Twitter](https://twitter.com/azuretrenches) - I might not get around to it immediately but I really appreciate it!

_Please note that Function Monkey, like Azure Functions v2, is still in preview and so is subject to breaking change. That said the public API for Function Monkey has settled down nicely at this point and is being used in a number of projects._



