using FunctionMonkey.Abstractions.Builders;
using OpenApi.Customers.CreateCustomer;
using System.Net.Http;

namespace OpenApi.Customers
{
    public static class FunctionBuilderExtension
    {
        public static IFunctionBuilder RegisterCustomers(this IFunctionBuilder functionBuilder)
        {
            functionBuilder.HttpRoute($"{Constants.HttpRoutePrefix}/Customers", http =>
            {
                http.HttpFunction<CreateCustomerHttpRequestDto>(HttpMethod.Post);
            });

            return functionBuilder;
        }
    }
}
