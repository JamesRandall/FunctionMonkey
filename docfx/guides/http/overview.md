# Overview

The aim of the HTTP trigger support in Function Monkey is to make it straightforward to support simple REST APIs running on Azure Functions and as such they are a little more involved than the other trigger types as in addition to the other framework features (validation in particular) they include support for:

* Routing
* Authentication
* Authorization
* Open API / Swagger
* Claims Mapping (binding claims to command properties)
* Headers

The aim isn't to replicate full blown ASP.Net Core (which is already supported on AWS Lambda and undoubtedly coming to Azure Functions at some point) but instead to focus on the common use cases and by doing so keep things lean and well suited to an environment where you pay per Gb/s.

As the [Getting Started](guides/gettingStarted.html) guide uses HTTP triggers as an example the rest of this guide will focus on the above areas in a little more depth.

## Routes and the "api" prefix

By default Azure Functions will prefix all HTTP routes with the word _api_. So if you define a route "myRoute/doSomething" it will actually be published on "api/myRoute/doSomething". You can't opt out of the prefix on a case by case basis and so I don't find this helpful, particularly when I'm defining APIs I generally want my Open API support to appear at "/openapi" and so I don't find this terribly helpful. To stop this from happening edit your host.json file so that it reads:

    {
        "version": "2.0",
        "extensions": {
            "http": {
                "routePrefix": ""
            }
        }
    }

If you don't do this just remember that all your routes will be prefixed with _api_.
