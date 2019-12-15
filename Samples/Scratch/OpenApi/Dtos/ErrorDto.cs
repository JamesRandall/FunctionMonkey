using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace OpenApi.Dtos
{
    /// <summary>
    /// Parameters of an error object
    /// </summary>
    [DataContract(Name = "Error")]
    public class ErrorDto
    {
        /// <summary>
        /// The unique identifier of this object.
        /// </summary>
        [Required]
        [StringLength(32, MinimumLength = 32)]
        [DataMember(Name = "id")]
        public string Id { get; set; }

        /// <summary>
        /// The status code of this error.
        /// </summary>
        [Required]
        [DataMember(Name = "code")]
        public int Code { get; set; }

        /// <summary>
        /// The error message.
        /// </summary>
        [Required]
        [StringLength(255)]
        [DataMember(Name = "message")]
        public string Message { get; set; }

        /// <summary>
        /// List of error details.
        /// </summary>
        [DataMember(Name = "details", EmitDefaultValue = false)]
        public IReadOnlyList<ErrorDetailDto> Details { get; set; }
    }
}
