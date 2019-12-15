using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace OpenApi.Dtos
{
    /// <summary>
    /// Parameters of an error detail object
    /// </summary>
    [DataContract(Name = "ErrorDetail")]
    public class ErrorDetailDto
    {
        /// <summary>
        /// The error detail message.
        /// </summary>
        [Required]
        [StringLength(255)]
        [DataMember(Name = "message")]
        public string Message { get; set; }

        /// <summary>
        /// List of fields.
        /// </summary>
        [DataMember(Name = "fields", EmitDefaultValue = false)]
        public IReadOnlyList<string> Fields { get; set; }
    }
}
