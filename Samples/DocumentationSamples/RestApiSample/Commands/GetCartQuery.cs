using System;
using AzureFromTheTrenches.Commanding.Abstractions;
using RestApiSample.Commands.Responses;

namespace RestApiSample.Commands
{
    public class GetCartQuery : ICommand<Cart>
    {
        [SecurityProperty]
        public Guid AuthenticatedUserId { get; set; }
    }
}
