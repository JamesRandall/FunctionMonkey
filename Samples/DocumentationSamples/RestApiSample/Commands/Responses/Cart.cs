using System;
using System.Collections.Generic;

namespace RestApiSample.Commands.Responses
{
    public class Cart
    {
        public Guid Id { get; set; }

        public Guid ShopperId { get; set; }

        public IReadOnlyCollection<CartItem> CartItems { get; set; }
    }
}
