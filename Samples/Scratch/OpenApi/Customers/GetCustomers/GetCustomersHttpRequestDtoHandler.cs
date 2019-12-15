using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace OpenApi.Customers.GetCustomers
{
    public class GetCustomersHttpRequestDtoHandler : ICommandHandler<GetCustomersHttpRequestDto, IActionResult>
    {
        public Task<IActionResult> ExecuteAsync(GetCustomersHttpRequestDto command, IActionResult previousResult)
        {
            throw new System.NotImplementedException();
        }
    }
}
