using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace OpenApi.Customers.CreateCustomer
{
    /// <summary>
    ///     The response DTO for the CreateCustomer request
    /// </summary>
    [DataContract(Name = "CreateCustomerHttpResponse")]
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class CreateCustomerHttpResponseDto
    {
        /// <summary>
        ///     The customer.
        /// </summary>
        [DataMember(Name = "customer")]
        public CustomerDto Customer { get; set; }
    }
}
