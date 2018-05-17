using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using SimpleRestApi.Model;
using SimpleRestApi.Queries;

namespace SimpleRestApi.Handlers
{
    internal class GetBlogPostQueryHandler : ICommandHandler<GetBlogPostQuery, BlogPost>
    {
        public Task<BlogPost> ExecuteAsync(GetBlogPostQuery command, BlogPost previousResult)
        {
            return Task.FromResult(new BlogPost
            {
                Body = "Our blog posts main text",
                PostId = command.PostId,
                Title = "Post Title"
            });
        }
    }
}
