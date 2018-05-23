# Command Handlers

A command handler contains the execution logic for it's associated type of command. Multiple handlers can be associated with a type of command and these can be arranged to run in a specific execution order - the execution of this set of handlers is referred to as the execution pipeline.

Command handlers are classes that implement one of the following interfaces each of which requires a single variant of _ExecuteAsync_ to be implemented:

|Interface|Description|
|---------|-----------|
|ICommandHandler&lt;in TCommand&gt;|Implemented for commands that require no result|
|ICancellableCommandHandler&lt;in TCommand&gt;|Implemented for commands that require no result. Implementations of this handler also have access to a cancellation token|
|ICommandHandler&lt;in TCommand, TResult>|Implemented for commands that require a result|
|ICancellableommandHandler&lt;in TCommand, TResult>|Implemented for commands that require a result. Implementations of this handler also have access to a cancellation token|
|IPipelineAwareCommandHandler&lt;in TCommand, TResult>|Implemented for commands that require a result and wish to be able to interact with the handler|
|ICancellablePipelineAwareCommandHandler&lt;in TCommand, TResult>|Implemented for commands that require a result and wish to be able to interact with the handler. Implementations of this handler also have access to a cancellation token.|

## Registering a Command Handler

Command handlers are registered with the ICommandRegistry interface. This is returned by the _UseCommanding()_ method, for example using the Microsoft.Extensions.DependencyInjector framework:

    IServiceCollection serviceCollection = new ServiceCollection();
    IMicrosoftDependencyInjectionCommandingResolver dependencyResolver = new MicrosoftDependencyInjectionCommandingResolver(serviceCollection);
    ICommandRegistry registry = dependencyResolver.UseCommanding(options);

A command handler can then be registered simply by providing the type:

    registry.Register<MyCommandHandler>();

Additionally an execution order can be supplied that determines the order within the execution pipeline - lower numbers will be executed first:

    registry.Register<FirstHandler>(1);
    registry.Register<SecondHandler>(2);

The order numbers do not nbeed to be sequential but if two command handlers have the same order their is no guarantee about the execution order.

An additional parameter is available during handler registration that primarily exists to enable custom command dispatch operations such as via HTTP. That is described in the section on [Custom Dispatchers](https://github.com/JamesRandall/AzureFromTheTrenches.Commanding/wiki/Custom-Command-Dispatchers).

Command handlers are constructed using the IoC container and so support all the construction and injection techniques supported by that container.

## Discovering Command Handlers

As a shortcut to register many similar command handlers the framework provides a Discover method. This will search a given set of assemblies for command handlers and register them. For example:

    Assembly myFirstHandlerAssembly = ...;
    Assembly mySecondHandlerAssembly = ...;

    registry.Discover(myFirstHandlerAssembly, mySecondHandlerAssembly);

Alternatively a type can be used as a reference for an assembly (the assembly that contains the type will be searched):

    registry.Discover<BasedOnType>();


## Examples of Command Handlers

Examples for each interface are shown below.

### ICommandHandler&lt;in TCommand&gt;

The simplest form of command handler simply returns a task:

    class MyCommandHandler : ICommandHandler<MyCommand>
    {
        public Task ExecuteAsync(MyCommand command)
        {
            Console.WriteLine("I'm doing something");
            return Task.FromResult(0);
        }
    }

### ICancellableCommandHandler&lt;in TCommand&gt;

This form of command handler builds on the above and also has access to a cancellation token if one was supplied during the dispatch call:

    class MyCommandHandler : ICancellableCommandHandler<MyCommand>
    {
        public Task ExecuteAsync(MyCommand command, CancellationToken cancellationToken)
        {
            Console.WriteLine("I'm doing something");
            return Task.FromResult(0);
        }
    }

### ICommandHandler<in TCommand, TResult>

When a command handler returns a result it also has access to the result returned by the previous handler in the pipeline.

    class MyCommandHandler : ICommandHandler<MyCommand, bool>
    {
        public Task<bool> ExecuteAsync(MyCommand command, bool previousResult)
        {
            Console.WriteLine("I'm doing something");
            return Task.FromResult(true);
        }
    }

### ICancellableCommandHandler<in TCommand, TResult>

Similar to the above but also has access to a cancellation token.

    class MyCommandHandler : ICancellableCommandHandler<MyCommand, bool>
    {
        public Task<bool> ExecuteAsync(MyCommand command, bool previousResult, CancellationToken cancellationToken)
        {
            Console.WriteLine("I'm doing something");
            return Task.FromResult(true);
        }
    }

### IPipelineAwareCommandHandler&lt;in TCommand, TResult>

A pipeline aware command handler returns an annotated result that states whether or not the pipeline should continue executing. This enables a handler to prevent subsequent handlers from running (for example in the event of a terminal error).

    class MyCommandHandler : IPipelineAwareCommandHandler<MyCommand, int>
    {
        public Task<PipelineAwareCommandHandlerResult<int>> ExecuteAsync(PipelineCommand command, bool previousResult)
        {
            Console.WriteLine("Pipeline command actor with cancellation token");
            // The below will stop execution of the pipeline due to the true passed to the constructor
            return Task.FromResult(new PipelineAwareCommandHandlerResult<bool>(true, 25));
        }
    }

The example above will stop the pipeline as true is supplied as the first parameter to the constructor of the _PipelineAwareCommandHandlerResult_ class, the second parameter is the result for the command. The example below will continue executing handlers:

    class MyCommandHandler : IPipelineAwareCommandHandler<MyCommand, int>
    {
        public Task<PipelineAwareCommandHandlerResult<int>> ExecuteAsync(PipelineCommand command, bool previousResult)
        {
            Console.WriteLine("Pipeline command actor with cancellation token");
            // The below will continue execution of the pipeline due to the true passed to the constructor
            return Task.FromResult(new PipelineAwareCommandHandlerResult<bool>(false, 25));
        }
    }

### ICancellablePipelineAwareCommandHandler&lt;in TCommand, TResult>

This operates the same as the previous interface but implementations are also supplied a cancellation token:

    class MyCommandHandler : ICancellablePipelineAwareCommandHandler<MyCommand, int>
    {
        public Task<PipelineAwareCommandHandlerResult<int>> ExecuteAsync(PipelineCommand command, bool previousResult, CancellationToken token)
        {
            Console.WriteLine("Pipeline command actor with cancellation token");
            // The below will continue execution of the pipeline due to the true passed to the constructor
            return Task.FromResult(new PipelineAwareCommandHandlerResult<bool>(false, 25));
        }
    }