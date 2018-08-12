using System;
using Microsoft.Extensions.DependencyInjection;

namespace FunctionMonkey.Abstractions
{
    public interface IContainerProvider
    {
        IServiceCollection CreateServiceCollection();

        IServiceProvider CreateServiceProvider(IServiceCollection serviceCollection);
    }
}
