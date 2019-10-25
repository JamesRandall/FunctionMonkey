using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace OpenApi.Customers
{
    /// <summary>
    ///     Parameters of a customer object
    /// </summary>
    [DataContract(Name = "Customer")]
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class CustomerDto
    {
        /// <summary>
        ///     The unique identifier of this customer object
        /// </summary>
        [DataMember(Name = "id")]
        public string Id { get; set; }

        /// <summary>
        ///     Fixed timestamp, recording when this customer object was created
        /// </summary>
        [DataMember(Name = "createdAt")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        ///     One of:
        ///     * created - The customer is created and its id can be immediately used in subsequent calls to the API.
        ///     * active - The customer has been successfully set up.
        ///     * failed - The customer could not be set up.
        ///     * discarded - The customer has been discarded.
        ///     * expired - The customer has expired due to dormancy.
        /// </summary>
        [DataMember(Name = "state")]
        public string State { get; set; }

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

        /// <summary>
        /// ISO 639-1 code. Used as the language for notification emails sent by SEPAexpress if 
        /// your organisation does not send its own (see compliance requirements). Currently 
        /// only “en”, “fr”, “de” are supported.
        /// Defaults to the country code of the address or "en" if not supported
        /// </summary>
        [MinLength(2)]
        [MaxLength(2)]
        [RegularExpression(@"(?i)^(en|de|fr)$")]
        [DataMember(Name = "languageCode", EmitDefaultValue = false)]
        public string LanguageCode { get; set; }
    }
}
