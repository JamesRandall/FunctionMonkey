using System;
using AzureFromTheTrenches.Commanding.Abstractions;
using SimpleRestApi.Model;

namespace SimpleRestApi.Queries
{
    public class GetBlogPostQuery : ICommand<BlogPost>
    {
        public Guid PostId { get; set; }
    }
}
