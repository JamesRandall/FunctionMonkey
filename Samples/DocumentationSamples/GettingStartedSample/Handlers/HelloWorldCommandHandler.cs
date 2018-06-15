using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using GettingStartedSample.Commands;
using GettingStartedSample.Services;

namespace GettingStartedSample.Handlers
{
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
}
