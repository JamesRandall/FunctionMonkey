# Logging

Function Monkey wires up the Azure Functions provided ILogger in the dependency injector.

To use it simply inject an _ILogger_ into your handlers or services.

If you add logging into your application via the _AddLogging_ extension method for _IServiceCollection_ that will replace the Azure Functions logger and you will need to ensure it outputs to targets as you require.

Handler using an injected logger:
```
public class GetEchoQueryHandler : ICommandHandler<GetEchoQuery>
    {
        private readonly ILogger<GetEchoQueryHandler> logger;

        public GetEchoQueryHandler(ILogger<GetEchoQueryHandler> logger)
        {
            this.logger = logger;
        }
        
        public Task ExecuteAsync(GetEchoQuery command)
        {
            this.logger.Log(LogLevel.Information, "echo");
            return Task.CompletedTask;
        }
     }
```

FunctionAppConfiguration logging setup:
```
 builder
                .Setup((serviceCollection, commandRegistry) =>
                {
                    ....
                    serviceCollection.AddLogging(c =>
                    {
                        c.AddConsole();
                        c.AddDebug();
                    });
```
