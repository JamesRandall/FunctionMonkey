using System.Collections.Generic;
using System.Runtime.Serialization;
using OpenApi.Dtos;

namespace OpenApi.Customers.GetCustomers
{
    /// <summary>
    ///     The response model for cursor-based paged customer list.
    /// </summary>
    [DataContract(Name = "GetCustomersHttpResponse")]
    public class GetCustomersHttpResponseDto
    {
        /// <summary>
        ///     Paging meta data
        /// </summary>

        [DataMember(Name = "meta")]
        public CursorPagedMetaDto Meta { get; set; }

        /// <summary>
        ///     List of customers.
        /// </summary>

        [DataMember(Name = "customers")]
        public IReadOnlyList<CustomerDto> Customers { get; set; }
    }
}
