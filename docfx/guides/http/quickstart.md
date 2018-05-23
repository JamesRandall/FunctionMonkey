# Quickstart

To understand this it's best to look at the [client sample](https://github.com/JamesRandall/AzureFromTheTrenches.Commanding/tree/master/Samples/HttpCommanding.Client) and the [server sample](https://github.com/JamesRandall/AzureFromTheTrenches.Commanding/tree/master/Samples/HttpCommanding.Web) however I'll cover getting started with this here. First you'll need to add the NuGet package:

    Install-Package AzureFromTheTrenches.Commanding.Http

Registration of a command is, optionally, slightly different if you want to use HTTP as you don't need to register an actor and you'll also need to install the HTTP handlers:

    resolver.AddHttpCommanding();
    registry.Register<AddCommand, MathResult>(HttpCommandDispatcherFactory.Create(uri, HttpMethod.Put));

There's nothing stopping you registering an actor too - but it won't do anything as the provided dispatcher will simply not execute actors locally (though it is possible to write a dispatcher that does execute locally and dispatch remotely). Note also above that you can specify both URI and HTTP verb for the command.

Dispatching the command is exactly the same as for in process immediate execution (other than that it will take a little longer):

    MathResult mathResult = await commandDispatcher.DispatchAsync(new AddCommand { FirstNumber = 5, SecondNumber = 6});
    Console.WriteLine(mathResult.Value);

Hopefully the above illustrates how you can change how commands are executed without changing your code, instead like with an IoC container, just the configuration.

To execute the commands as part of a HTTP based API then you have two choices. The recommended approach is to use the ASP.Net Core extensions that you can [read about here](/guides/restApi/quickstart.md).

Alternatively you can take a more low level approach by injecting an instance of _IDirectCommandExecuter_ into your controller and executing the command directly (you could also use the _ICommandDispatcher_ interface if you wish to take advantage of its broader functionality, including further dispatch, however _ICommandExecuter_ picks up where dispatch leaves off and therefore is marginally more optimal). In ASP.Net Core MVC this looks like the below:

    [Route("api/[controller]")]
    public class AddController : Controller
    {
        private readonly IDirectCommandExecuter _commandExecuter;

        public AddController(IDirectCommandExecuter commandExecuter)
        {
            _commandExecuter = commandExecuter;
        }

        [HttpPut]
        public async Task<MathResult> Put([FromBody]AddCommand command)
        {
            UpdateResult result = await _commandExecuter.ExecuteAsync<AddCommand, MathResult>(command);
            return result;
        }        
    }

There's nothing special about the configuration on the server side. In fact it looks exactly like our in memory example from earlier but simply sits in the _ConfigureServices_ method of the ASP.Net Core startup class:

    public void ConfigureServices(IServiceCollection services)
    {
        // Add framework services.
        services.AddMvc();
        ICommandingDependencyResolverAdapter adapter = new CommandingDependencyResolverAdapter(
            (type, instance) => serviceCollection.AddSingleton(type, instance),
            (type, impl) => serviceCollection.AddTransient(type, impl),
            type => serviceProvider.GetService(type)
        );
        adapter
            .AddCommanding()
            .Register<UpdatePersonalDetailsCommand, UpdatePersonalDetailsCommandActor>();
    }

By default the built in serializer use JSON however you can supply an alternative serializer by implementing the IHttpCommandSerializer interface and registering it when you install HTTP commanding, for example:

    resolver.AddHttpCommanding<MyCustomSerializer>();

The IHttpCommandSerializer interface defines methods for serializing, deserializing and stating the MIME / media type.
