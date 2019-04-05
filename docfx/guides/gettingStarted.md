# Getting Started

First begin by creating an empty Azure Functions v2 project and then install the core nuget packages for Function Monkey:

    Install-Package FunctionMonkey
    Install-Package FunctionMonkey.Compiler

The first package contains the core framework while the second will add a custom build step to your solution that generates the necessary assets required by the Azure Functions v2 host.

As an example we'll create a simple HTTP triggered function that when given a name returns a simple hello world message as a response.

Function Monkey is based on the mediator pattern and you can find more information about this in a series on [my blog here](https://www.azurefromthetrenches.com/c-cloud-application-architecture-commanding-with-a-mediator-the-full-series/) or in the [documentation for the mediator framework](https://commanding.azurefromthetrenches.com) used by Function Monkey. In short this works by defining commands as simple C# objects and then implementing command handlers that respond to those commands and optionally return a response.

By default Azure Functions will prefix all HTTP routes with the word _api_. So if you define a route "myRoute/doSomething" it will actually be published on "api/myRoute/doSomething". When I'm defining APIs I generally want my Open API support to appear at "/openapi" and so I don't find this terribly helpful. To stop this from happening edit your host.json file so that it reads:

    {
        "version": "2.0",
        "extensions": {
            "http": {
                "routePrefix": ""
            }
        }
    }

Now lets begin by creating a folder in our project called Commands and in there we'll add a class called HelloWorldCommand: 

    public class HelloWorldCommand : ICommand<string>
    {
        public string Name { get; set; }
    }

The interface ICommand<TResponse> has no implementation required but tags this class as a command that responds with a string. Commands can return complex types, value types, or be response-less. Its important that your command class is a public class - it will be accessed directly from another assembly.

Next lets add a folder in our project called Handlers and in there we'll add a class called HelloWorldCommandHandler:

    internal class HelloWorldCommandHandler : ICommandHandler<HelloWorldCommand, string>
    {
        public Task<string> ExecuteAsync(HelloWorldCommand command, string previousResult)
        {
            if (string.IsNullOrWhiteSpace(command.Name))
            {
                return Task.FromResult("Hello stranger");
            }
            return Task.FromResult($"Hello {command.Name}");
        }
    }

This handler is defined, by the interface implementation, as processing commands of type HelloWorldCommand that return strings. Unlike the command this can happily be an internal class - it will be instantiated through dependency injection.

Having defined our command and a handler all that remains is to map them onto a function trigger. We do that by creating a public class that implements the _IFunctionAppConfiguration_ interface. Add a class in our project root called FunctionAppConfiguration like that below:

    public class FunctionAppConfiguration : IFunctionAppConfiguration
    {
        public void Build(IFunctionHostBuilder builder)
        {
            builder
                .Setup((serviceCollection, commandRegistry) =>
                {
                    commandRegistry.Register<HelloWorldCommandHandler>();
                })
                .Functions(functions => functions
                    .HttpRoute("v1/HelloWorld", route => route
                        .HttpFunction<HelloWorldCommand>()
                    )
                );
        }
    }

Our class needs to implement the Build method and within it we do some basic setup and then define our triggers. In the setup phase we register our command handler with the mediation system (this can also be done through a discovery approach if you have many handlers but for the purposes of an example this is more explicit). In this block we could also register any other dependencies we require to be available.

Next we define our Azure Functions using the Functions block - HTTP functions can be grouped by route and in this example we define a single route available at /v1/HelloWorld and register a single function against that route that responds to our _HelloWorldCommand_. By default this will be available as a HTTP GET operation with no additional routing information (for more options see the section on HTTP triggers).

Ok. We can now run the project! If you do so you should see the Azure Function host startup and present two Http Functions, you should see something like this at the bottom of the console window:

    Http Functions:

        HelloWorld: http://localhost:7071/v1/HelloWorld

If you open up your browser and enter the URL:

    http://localhost:7071/v1/HelloWorld

Then the browser should show the content:

    "Hello stranger"

And if you enter the URL:

    http://localhost:7071/v1/HelloWorld?name=James

You should see the message:

    "Hello James"

Let's build this out a little and implement a simple dependency that creates a hash of a name. First create a folder called Services and in it add an interface called IStringHasher:

    internal interface IStringHasher
    {
        string Hash(string value);
    }

And then add an implementation class:

    internal class StringHasher : IStringHasher
    {
        public string Hash(string value)
        {
            SHA256 sha256 = new SHA256Managed();
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(value));
            string result = Convert.ToBase64String(bytes);
            return result;
        }
    }

Now lets update our command handler to take this as a constructor dependency and use it in the output:

    internal class HelloWorldCommandHandler : ICommandHandler<HelloWorldCommand, string>
    {
        private readonly IStringHasher _stringHasher;

        public HelloWorldCommandHandler(IStringHasher stringHasher)
        {
            _stringHasher = stringHasher;
        }

        public Task<string> ExecuteAsync(HelloWorldCommand command, string previousResult)
        {
            if (string.IsNullOrWhiteSpace(command.Name))
            {
                return Task.FromResult("Hello stranger");
            }
            return Task.FromResult($"Hello {command.Name}, from now on I'm going to call you {_stringHasher.Hash(command.Name)}");
        }
    }

And all that's left to do is register our dependency with our IoC container in the _FunctionAppConfiguration_ class which now looks like this:

    public class FunctionAppConfiguration : IFunctionAppConfiguration
    {
        public void Build(IFunctionHostBuilder builder)
        {
            builder
                .Setup((serviceCollection, commandRegistry) =>
                {
                    // Add the line below
                    serviceCollection.AddTransient<IStringHasher, StringHasher>();
                    commandRegistry.Register<HelloWorldCommandHandler>();
                })
                .Functions(functions => functions
                    .HttpRoute("v1/HelloWorld", route => route
                        .HttpFunction<HelloWorldCommand>()
                    )
                );
        }
    }

If you run that and open the URL:

    http://localhost:7071/v1/HelloWorld?name=James

You should now see a message like this:

    "Hello James, from now on I'm going to call you k0WjWm/fF03/chkoKjrkh5eQ27eFxw9v/5HjL6/Wbqs="

Finally lets add one more twist and introduce some validation - we'll the application so that a name of between 1 and 50 characters in length is required. First we'll need to add a new NuGet package:

    Install-Package FunctionMonkey.FluentValidation

Now add a Validators folder to the root of the project and in there create a class called _HelloWorldCommandValidator_:

    internal class HelloWorldCommandValidator : AbstractValidator<HelloWorldCommand>
    {
        public HelloWorldCommandValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MinimumLength(1).MaximumLength(50);
        }
    }

Although you can use any validation system with Function Monkey (see the section on validation) the default package makes use of the excellent [FluentValidation](https://github.com/JeremySkinner/FluentValidation) framework and the above is a pretty standard validator for that system. Next we need to register it with out IoC container and add the Fluent Validation system so we'll update our _FunctionAppConfiguration_ class to look like the below:

    public class FunctionAppConfiguration : IFunctionAppConfiguration
    {
        public void Build(IFunctionHostBuilder builder)
        {
            builder
                .Setup((serviceCollection, commandRegistry) =>
                {
                    // Add the line below
                    serviceCollection.AddTransient<IValidator<HelloWorldCommand>, HelloWorldCommandValidator>();
                    serviceCollection.AddTransient<IStringHasher, StringHasher>();
                    commandRegistry.Register<HelloWorldCommandHandler>();
                })
                // And add the line below
                .AddFluentValidation()
                .Functions(functions => functions
                    .HttpRoute("v1/HelloWorld", route => route
                        .HttpFunction<HelloWorldCommand>()
                    )
                );
        }
    }

If you run the project and open the below URL in a browser or Postman:

    http://localhost:7071/v1/HelloWorld

Then you should see a validation block returned that looks like this:

    {
        "errors": [
            {
            "severity": 0,
            "errorCode": "NotEmptyValidator",
            "property": "Name",
            "message": "'Name' should not be empty."
            }
        ],
        "isValid": false
    }

Its worth noting that validation is applied to all trigger types so you can use it with (none stream based) blobs, queues, etc.

The source code for the above can be found over in [GitHub](https://github.com/JamesRandall/FunctionMonkey/tree/master/Samples/DocumentationSamples/GettingStartedSample).

Obviously for something so simple this is a little bit contrived - but if you imagine growing this out over time and
adding more functions hopefully its clear how this approach helps keep code cleaner - and not only that your implementation is completely decoupled from Azure Functions itself so if you wanted to run them within another context all you need do is wire up the command handling (take a look at the underlying mediator framework documentation for how to go about that).

If this has whetted your appetite then I suggest looking at the more detailed documentation for the currently supported trigger types (growing all the time!) on the left which will also cover the more advanced options available.