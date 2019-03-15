# IoC Containers

By default Function Monkey makes use of the Microsoft ASP.Net Core container as represented by the IServiceCollection and IServiceProvider interfaces. It is possible to use a different IoC container as long as it supports these abstractions - essentially if its compatible with ASP.Net Core.

To use a different container your function app configuration class also needs to implement the _IContainerProvider_ interface. I'll demonstrate how this works using the _FunctionAppConfiguration_ class presented in the getting started guide:

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

Lets assume we want to use Autofac instead of the Microsoft implementation. First we need to add the required Autofac packages to our project:

    Install-Package Autofac
    Install-Package Autofac.Extensions.DependencyInjection

Then we update our FunctionAppConfiguration class to also implement the _IContainerProvider_ interface has shown below:

    public class FunctionAppConfiguration : IFunctionAppConfiguration, IContainerProvider
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

        public IServiceCollection CreateServiceCollection()
        {
            return new ServiceCollection();
        }

        public IServiceProvider CreateServiceProvider(IServiceCollection serviceCollection)
        {
            ContainerBuilder containerBuilder = new ContainerBuilder();
            containerBuilder.Populate(serviceCollection);
            IContainer autofacContainer = containerBuilder.Build();
            IServiceProvider serviceProvider = new AutofacServiceProvider(autofacContainer);
            return serviceProvider;
        }
    }

Note that if you want to take advantage of container capabilities that are not represented in the Microsoft abstractions then you can register things here in addition to Setup.