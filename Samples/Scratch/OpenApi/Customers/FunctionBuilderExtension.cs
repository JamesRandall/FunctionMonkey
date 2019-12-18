using System.Net;
using FunctionMonkey.Abstractions.Builders;
using OpenApi.Customers.CreateCustomer;
using System.Net.Http;
using OpenApi.Customers.GetCustomers;
using OpenApi.Dtos;

namespace OpenApi.Customers
{
    public static class FunctionBuilderExtension
    {
        public static IFunctionBuilder RegisterCustomers(this IFunctionBuilder functionBuilder)
        {
            functionBuilder.HttpRoute($"{Constants.HttpRoutePrefix}/Customers", http =>
            {
                http.HttpFunction<CreateCustomerHttpRequestDto>(HttpMethod.Post)
                    .OpenApiSummary("Creates a new customer.")
                    .OpenApiDescription("Creates a new customer. The newly created resource can be referenced by the URI given by a Location header field in the response.")
                    .OpenApiResponse((int)HttpStatusCode.Created, "Created. The request has been fulfilled and resulted in a new resource being created.", typeof(CreateCustomerHttpResponseDto))
                    .OpenApiResponse((int)HttpStatusCode.BadRequest, "Bad Request. The request could not be understood by the server, usually due to malformed syntax.", typeof(ErrorResponseDto))
                    .OpenApiResponse((int)HttpStatusCode.Unauthorized, "Unauthorized. The client has not provided a valid Authentication HTTP header or the user making the request has been disabled.", typeof(ErrorResponseDto))
                    .OpenApiResponse((int)HttpStatusCode.Forbidden, "Forbidden. The client has provided a valid Authentication header, but does not have permission to access this resource.", typeof(ErrorResponseDto))
                    .OpenApiResponse((int)HttpStatusCode.Conflict, "Conflict. The resource to be created by your request already exists.", typeof(ErrorResponseDto))
                    .OpenApiResponse((int)HttpStatusCode.TooManyRequests, "Too Many Requests. A rate limit has been reached.", typeof(ErrorResponseDto))
                    ;
                http.HttpFunction<GetCustomersHttpRequestDto>(HttpMethod.Get)
                    .OpenApiSummary("Returns a cursor paged list of customers.")
                    .OpenApiDescription("Returns a cursor paged list of customers.")
                    .OpenApiResponse((int)HttpStatusCode.OK, "Successful request.", typeof(GetCustomersHttpResponseDto))
                    .OpenApiResponse((int)HttpStatusCode.BadRequest, "Bad Request. The request could not be understood by the server, usually due to malformed syntax.", typeof(ErrorResponseDto))
                    .OpenApiResponse((int)HttpStatusCode.Unauthorized, "Unauthorized. The client has not provided a valid Authentication HTTP header or the user making the request has been disabled.", typeof(ErrorResponseDto))
                    .OpenApiResponse((int)HttpStatusCode.Forbidden, "Forbidden. The client has provided a valid Authentication header, but does not have permission to access this resource.", typeof(ErrorResponseDto))
                    .OpenApiResponse((int)HttpStatusCode.TooManyRequests, "Too Many Requests. A rate limit has been reached.", typeof(ErrorResponseDto))
                    ;
            });

            return functionBuilder;
        }
    }
}
