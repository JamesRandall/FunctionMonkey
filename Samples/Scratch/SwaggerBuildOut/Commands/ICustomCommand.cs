using System;
using System.Collections.Generic;
using System.Text;
using AzureFromTheTrenches.Commanding.Abstractions;

namespace SwaggerBuildOut.Commands
{
    public interface ICustomCommand<T> : ICommand<T>
    {
    }
}
