using System;
using System.Runtime.Serialization;
using AzureFromTheTrenches.Commanding.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace OpenApi.Customers.GetCustomers
{
    /// <summary>
    ///     The request DTO for the GetCustomers request
    /// </summary>
    [DataContract(Name = "GetCustomersHttpRequest")]
    public class GetCustomersHttpRequestDto : ICommand<IActionResult>
    {
        /// <summary>
        ///     Return only objects after this cursor id. Optional 
        /// </summary>
        [DataMember(Name = "after")]
        public string After { get; set; }

        /// <summary>
        ///     Return only objects before this cursor id. Optional
        /// </summary>
        [DataMember(Name = "before")]
        public string Before { get; set; }

        /// <summary>
        ///     Upper bound for the number of objects to be returned. Defaults to 50. Maximum of 500. Optional
        /// </summary>
        [DataMember(Name = "limit")]
        public int? Limit { get; set; } = 50;

        /// <summary>
        ///     Return only objects which were created from this timestamp on. Optional. Remark: Do not use timestamps for the
        ///     paging of search results as there can be multiple objects with the same timestamp.
        /// </summary>
        [DataMember(Name = "createdFrom")]
        public DateTime? CreatedFrom { get; set; }

        /// <summary>
        ///     Return only objects which were created until this timestamp. Optional. Remark: Do not use timestamps for the paging
        ///     of search results as there can be multiple objects with the same timestamp.
        /// </summary>
        [DataMember(Name = "createdUntil")]
        public DateTime? CreatedUntil { get; set; }
    }
}
