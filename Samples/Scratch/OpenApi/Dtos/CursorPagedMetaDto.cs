using System.Runtime.Serialization;

namespace OpenApi.Dtos
{
    /// <summary>
    /// Paging meta data
    /// </summary>
    [DataContract(Name = "CursorPagedMeta")]
    public class CursorPagedMetaDto
    {
        /// <summary>
        /// The unique identifier of the first item that has been returned.
        /// </summary>
        [DataMember(Name = "before", EmitDefaultValue = false)]
        public string Before { get; set; }

        /// <summary>
        /// The unique identifier of the last item that has been returned.
        /// </summary>
        [DataMember(Name = "after", EmitDefaultValue = false)]
        public string After { get; set; }

        /// <summary>
        /// The upper bound placed on the number of items returned. If there were not enough 
        /// remaining items in the list of data then fewer than this number will have been returned.
        /// </summary>
        [DataMember(Name = "limit")]
        public int Limit { get; set; }

        /// <summary>
        /// The number of items returned.
        /// </summary>
        [DataMember(Name = "count")]
        public int Count { get; set; }
    }
}
