using AzureFromTheTrenches.Commanding.Abstractions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace OpenApi.Customers.CreateCustomer
{
    public class CreateCustomerHttpRequestDtoHandler : ICommandHandler<CreateCustomerHttpRequestDto, IActionResult>
    {
        public Task<IActionResult> ExecuteAsync(CreateCustomerHttpRequestDto command, IActionResult previousResult)
        {
            throw new NotImplementedException();
        }
    }
}
