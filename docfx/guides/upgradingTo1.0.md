# Upgrading to 1.0

If you're using a beta release there are a couple of changes required to adopt the 1.0 and later releases.

## HTTP Triggers

### Routes

In versions prior to 1.0 Function Monkey relied on proxies to shape HTTP trigger endpoints into friendly routes - the reasons why are lost to the mists of time I'm afraid!

In version 1.0 the route property is used on the HttpTriggerAttribute and in the function.json as per [this guidance here](https://docs.microsoft.com/en-us/azure/azure-functions/functions-bindings-http-webhook#customize-the-http-endpoint).

That does mean that by default 'api' will be added anbd used as a prefix for all your routes by the Azure Functions runtime and that will require you to address this in your routes. The easiest way to update this is simply to remove the prefix by editing the host.json file as shown in the example below:

    {
      "version": "2.0",
      "extensions": {
        "http": {
          "routePrefix": ""
        }
      }
    }

### Response Handlers

Previously it was not possible to use a response handler to shape the output of validation failures - this is addressed in version 1.0 by the addition of the _CreateValidationFailureResponse_ method to the _IHttpResponseHandler_ interface.

If you've created custom response handlers you will need to implement this method on your classes, as per the other methods you can return null to have Function Monkey create its default response, or you can return an _IActionResult_ to tailor the response.
