using AzureFromTheTrenches.Commanding.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace OpenApi.Customers.CreateCustomer
{
    /// <summary>
    ///     The request DTO for the CreateCustomer request
    /// </summary>
    [DataContract(Name = "CreateCustomerHttpRequest")]
    public class CreateCustomerHttpRequestDto : ICommand<IActionResult>
    {
        /// <summary>
        ///     Customer’s given name. Required unless a company name is provided
        /// </summary>
        [DataMember(Name = "givenName")]
        public string GivenName { get; set; }

        /// <summary>
        ///     Customer’s family name. Required unless a company name is provided
        /// </summary>
        [DataMember(Name = "familyName")]
        public string FamilyName { get; set; }

        /// <summary>
        ///     Customer’s company name. Required unless a given name and family name are provided
        /// </summary>
        [DataMember(Name = "companyName")]
        public string CompanyName { get; set; }
    }
}
