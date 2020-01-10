using System.Threading;
using System.Threading.Tasks;

namespace FunctionMonkey.Abstractions
{
    /// <summary>
    /// Buffers Function Monkey from the underlying mediator implementation
    /// </summary>
    public interface IMediatorDecorator
    {
        Task<TResult> RequestAsync<TResult>(object command, CancellationToken token);

        Task SendAsync(object command, CancellationToken token);
    }
}