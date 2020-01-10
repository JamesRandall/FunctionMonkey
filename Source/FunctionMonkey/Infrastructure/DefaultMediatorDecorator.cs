using System.Threading;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Abstractions;

namespace FunctionMonkey.Infrastructure
{
    internal class DefaultMediatorDecorator : IMediatorDecorator
    {
        private readonly ICommandDispatcher _dispatcher;

        public DefaultMediatorDecorator(ICommandDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        public async Task<TResult> RequestAsync<TResult>(object command, CancellationToken token)
        {
            var dispatchResult = await _dispatcher.DispatchAsync((ICommand<TResult>) command, token);
            return dispatchResult.Result;
        }

        public Task SendAsync(object command, CancellationToken token)
        {
            return _dispatcher.DispatchAsync((ICommand) command, token);
        }
    }
}