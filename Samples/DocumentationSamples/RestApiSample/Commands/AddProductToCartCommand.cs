using System;
using AzureFromTheTrenches.Commanding.Abstractions;
using RestApiSample.Commands.Responses;

namespace RestApiSample.Commands
{
    public class AddProductToCartCommand : ICommand<Cart>
    {
        [SecurityProperty]
        public Guid AuthenticatedUserId { get; set; }

        public Guid ProductId { get; set; }

        public int Quantity { get; set; }
    }
}
