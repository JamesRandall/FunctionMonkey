# Logging

Function Monkey wires up the Azure Functions provided ILogger in the dependency injector.

To use it simply inject an _ILogger_ into your handlers or services.

If you add logging into your application via the _AddLogging_ extension method for _IServiceCollection_ that will replace the Azure Functions logger and you will need to ensure it outputs to targets as you require.

