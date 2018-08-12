using System;
using FunctionMonkey.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace FunctionMonkey.Infrastructure
{
    class DefaultContainerProvider : IContainerProvider
    {
        public IServiceCollection CreateServiceCollection()
        {
            return new ServiceCollection();
        }

        public IServiceProvider CreateServiceProvider(IServiceCollection serviceCollection)
        {
            return serviceCollection.BuildServiceProvider();
        }
    }
}
