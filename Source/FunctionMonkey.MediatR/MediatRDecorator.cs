using System;
using System.Threading;
using System.Threading.Tasks;
using FunctionMonkey.Abstractions;
using MediatR;

namespace FunctionMonkey.MediatR
{
    internal class MediatRDecorator : IMediatorDecorator
    {
        private readonly IMediator _mediator;

        public MediatRDecorator(IMediator mediator)
        {
            _mediator = mediator;
        }
        
        public async Task<TResult> RequestAsync<TResult>(object command, CancellationToken token)
        {
            TResult result = await _mediator.Send((IRequest<TResult>) command, token);
            return result;
        }

        public async Task SendAsync(object command, CancellationToken token)
        {
            await _mediator.Publish(command, token);
        }
    }
}