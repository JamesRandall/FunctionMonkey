using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;

namespace NetCore21Example
{
    class SimpleCommandHandler : ICommandHandler<SimpleCommand>
    {
        public Task ExecuteAsync(SimpleCommand command)
        {
            throw new NotImplementedException();
        }
    }
}
