# Configuration Options

The _AddCommanding()_ method can be supplied an optional Options object to further configure the commanding system. For example:

    var options = new Options() { CommandActorContainerRegistration = type => services.AddTransient(type, type) };
    resolver.AddCommanding(options);

The options class has the following properties:

|Property|Description|
|--------|-----------|
|AuditItemEnricherFactoryFunc|By default audit item enrichers are created through the dependency resolver but if a function is assigned to the AuditItemEnricherFactoryFunc property then that function will be used to instantiate an enricher. See [Auditing and Event Sourcing](auditing.md).|
|CommandExecutionExceptionHandler|Can be used to register a type that implements the ICommandExecutionExceptionHandler interface that will be invoked in the event of an execution error. See [error handling](errorHandling.md).|
|CommandHandlerContainerRegistration|Unless an alternative implementation of ICommandHandlerFactory is supplied or the CommandHandlerFactoryFunc in this options class is set then handlers are created through the dependency resolver but not all IoC containers can resolve unregistered concrete types (for example the built in ASP.Net Core IServiceCollection and IServiceProvider IoC cannot). Where this is the case supply an implementation for the CommandHandlerContainerRegistration action that registers the handlers in the container. For example using an IServiceCollection instance of serviceCollection:<br/>`var options = new Options() { CommandHandlerContainerRegistration = type => services.AddTransient(type, type) };`|
|CommandHandlerFactoryFunc|By default handlers are created through the dependency resolver but if a function is assigned to the CommandHandlerFactoryFunc property then that function will be called to instantiate a handler.|
|DisableCorrelationIds|Setting this to true will disable all correlatin ID generation which can give a small performance boost in super-low latency situations if you're prepared to trade off correlation.|
|Enrichers|It can be useful to include additional properties in the command context, for example the Application Insights Operation ID. This can be done by setting this property to a set of enrichment functions - these functions are called in sequence and are passed the current state of the command contexts enriched properties and can return a dictionary of properties to insert. If the returned dictionary contains a property that already exists it will be replaced in the command context property bag.|
|MetricCollectionEnabled|Set to false to disable timing metrics around command dispatch and execution and gain a minor performance gain in very high throughput local execution scenarios where you are trying to squeeze out fractions of milliseconds|
|Reset|The commanding system maintains command registrations, command context enrichers, and auditors between calls to UseCommanding to enable sub-modules to extend the system without any awareness / tight coupling between them. If you want to force a reset of the shared state then set this property to true (generally this is only useful in sample apps where you might repeatedly recreate and reconfigure an IoC container as the included samples do.|
|UseLocallyUniqueCorrelationIds|By default correlation IDs will be generated as GUIDs so that they are unique across as distributed system however if performance is key a local;y unique (incrementing long, thread safe) correlation ID can be used by setting this to true|