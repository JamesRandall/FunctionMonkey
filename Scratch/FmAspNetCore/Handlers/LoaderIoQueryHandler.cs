using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FmAspNetCore.Commands;

namespace FmAspNetCore.Handlers
{
    public class LoaderIoQueryHandler : ICommandHandler<LoaderIoQuery, string>
    {
        public Task<string> ExecuteAsync(LoaderIoQuery command, string previousResult)
        {
            return Task.FromResult("loaderio-fa4864b7cff0c92b67ffdc6c1b85d9a5");
        }
    }
}