# Getting Started

Firstly begin by creating an empty Azure Functions v2 project and then install the core nuget packages for Function Monkey:

    Install-Package FunctionMonkey
    Install-Package FunctionMonkey.Compiler

The first package contains the core framework while the second will add a custom build step to your solution that generates the necessary assets required by the Azure Functions v2 host.

As an example we'll create a simple HTTP triggered function that when given a name returns a simple hello world message as a response.

Function Monkey is based on the mediator pattern and you can find more information about this in a series on [my blog here](https://www.azurefromthetrenches.com/c-cloud-application-architecture-commanding-with-a-mediator-the-full-series/) or in the [documentation for the mediator framework](https://commanding.azurefromthetrenches.com) used by Function Monkey. In short this works by defining commands as simple C# objects and then implementing command handlers that respond to those commands and optionally return a response.

Lets begin by creating a folder in our project called Commands and in there we'll add a class called HelloWorldCommand: 

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
                    .HttpRoute("/api/v1/HelloWorld", route => route
                        .HttpFunction<HelloWorldCommand>()
                    )
                );
        }
    }

Our class needs to implement the Build method and within it we do some basic setup and then define our triggers. In the setup phase we register our command handler with the mediation system (this can also be done through a discovery approach if you have many handlers but for the purposes of an example this is more explicit). In this block we could also register any other dependencies we require to be available.

Next we define our Azure Functions using the Functions block - HTTP functions can be grouped by route and in this example we define a single route available at /api/v1/HelloWorld and register a single function against that route that responds to our _HelloWorldCommand_. By default this will be available as a HTTP GET operation with no additional routing information (for more options see the section on HTTP triggers).

We have one final step to take before we can run this - in order to present nicely routed REST APIs Function Monkey generates Azure Function Proxies and these need to know where the underlying function(s) live - including the domain. If you've followed along with a default Azure Functions setup on a development box then these will be available at http://localhost:7071. In order to make this play nicely in different environments an environment variable called ProxyPrefix needs to be set in local.settings.json (and if you deploy this to Azure in the Functions App's app settings) as shown below (based on the default local.settings.json file):

    {
      "IsEncrypted": false,
      "Values": {
        "AzureWebJobsStorage": "UseDevelopmentStorage=true",
        "AzureWebJobsDashboard": "UseDevelopmentStorage=true",
        "ProxyPrefix": "http://localhost:7071"
      }
    }

Ok. We can now run the project! If you do so you should see the Azure Function host startup and present two Http Functions, you should see something like this at the bottom of the console window:

    Http Functions:

        HelloWorldProxy: http://localhost:7071/api/HelloWorldProxy

        HelloWorld: http://localhost:7071/api/HelloWorld

One of those is our underlying function and the other the proxy. If you open up your browser and enter the URL:

    http://localhost:7071/api/v1/HelloWorld

Then the browser should show the content:

    "Hello stranger"

And if you enter the URL:

    http://localhost:7071/api/v1/HelloWorld?name=James

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
                    serviceCollection.AddTransient<IStringHasher, StringHasher>();
                    commandRegistry.Register<HelloWorldCommandHandler>();
                })
                .Functions(functions => functions
                    .HttpRoute("/api/v1/HelloWorld", route => route
                        .HttpFunction<HelloWorldCommand>()
                    )
                );
        }
    }

If you run that and open the URL:

    http://localhost:7071/api/v1/HelloWorld?name=James

You should now see a message like this:

    "Hello James, from now on I'm going to call you k0WjWm/fF03/chkoKjrkh5eQ27eFxw9v/5HjL6/Wbqs="

And for simple usage that's it!

Obviously for something so simple this is a little bit contrived - but if you imagine growing this out over time and
adding more functions hopefully its clear how this approach helps keep code cleaner - and not only that your implementation is completely decoupled from Azure Functions itself so if you wanted to run them within another context all you need do is wire up the command handling (take a look at the underlying mediator framework documentation for how to go about that).

If this has whetted your appetite then I suggest looking at the more detailed documentation for the currently supported trigger types (growing all the time!) on the left which will also cover the more advanced options available.