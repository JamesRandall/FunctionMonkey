using System;
using AzureFromTheTrenches.Commanding.Abstractions;
using RestApiSample.Commands.Responses;

namespace RestApiSample.Commands
{
    public class CreateOrderFromCartCommand  : ICommand<Order>
    {
        [SecurityProperty]
        public Guid AuthenticatedUserId { get; set; }
    }
}
