using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace OpenApi.Dtos
{
    /// <summary>
    /// The response DTO for an erroneous request
    /// </summary>
    [DataContract(Name = "ErrorResponse")]
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class ErrorResponseDto
    {
        /// <summary>
        /// The error.
        /// </summary>
        [Required]
        [DataMember(Name = "error")]
        public ErrorDto Error { get; set; }
    }
}
