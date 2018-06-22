using System;

namespace RestApiSample.Commands.Responses
{
    public class CartItem
    {
        public Guid ProductId { get; set; }

        public int Quantity { get; set; }
    }
}
