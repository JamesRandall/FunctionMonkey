using System;
using AzureFromTheTrenches.Commanding.Abstractions;

namespace RestApiSample.Commands
{
    public class CreateOrderFromCartCommand  : ICommand<Order>
    {
        [SecurityProperty]
        public Guid AuthenticatedUserId { get; set; }
    }
}
