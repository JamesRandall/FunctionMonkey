using System;
using System.Collections.Generic;
using System.Linq;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Abstractions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace FunctionMonkey.Serialization
{
    /// <summary>
    /// Json serializer that honours security properties and uses the supplied naming strategy
    /// </summary>
    public class NamingStrategyJsonSerializer : ISerializer
    {
        private readonly NamingStrategy _deserializerNamingStrategy;
        private readonly NamingStrategy _serializerNamingStrategy;
        private readonly IList<JsonConverter> _jsonConverters;
        
        /// <summary>
        /// Constructor
        /// </summary>
        public NamingStrategyJsonSerializer(NamingStrategy namingStrategy)
        {
            _deserializerNamingStrategy = namingStrategy ?? throw new ArgumentNullException(nameof(namingStrategy));
            _serializerNamingStrategy = namingStrategy;
        }
        
        /// <summary>
        /// Constructor
        /// </summary>
        public NamingStrategyJsonSerializer(NamingStrategy namingStrategy, IEnumerable<JsonConverter> converters)
        {
            _deserializerNamingStrategy = namingStrategy ?? throw new ArgumentNullException(nameof(namingStrategy));
            _serializerNamingStrategy = namingStrategy;
            _jsonConverters = converters.ToList();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="deserializerNamingStrategy">The naming strategy to use for deserialization</param>
        /// <param name="serializerNamingStrategy">The naming strategy to use for serialization</param>
        public NamingStrategyJsonSerializer(NamingStrategy deserializerNamingStrategy, NamingStrategy serializerNamingStrategy)
        {
            _deserializerNamingStrategy = deserializerNamingStrategy ?? throw new ArgumentNullException(nameof(deserializerNamingStrategy));
            _serializerNamingStrategy = serializerNamingStrategy;
        }
        
        /// <summary>
        /// Deserialize the command from the supplied json
        /// </summary>
        /// <param name="json">The json</param>
        /// <param name="enforceSecurityProperties">True if [SecurityProperty] should be honoured (not used in deserializtion)</param>
        /// <typeparam name="TCommand">The type</typeparam>
        /// <returns>The deserialized object</returns>
        public TCommand Deserialize<TCommand>(string json, bool enforceSecurityProperties)
        {
            JsonSerializerSettings serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = enforceSecurityProperties
                    ? new JsonSecurityPropertyContractResolver() {NamingStrategy = _deserializerNamingStrategy}
                    : new DefaultContractResolver() {NamingStrategy = _deserializerNamingStrategy}
            };
            try
            {
                return JsonConvert.DeserializeObject<TCommand>(json, serializerSettings);
            }
            catch (JsonReaderException ex)
            {
                throw new DeserializationException(ex.Path, ex.LineNumber, ex.LinePosition);
            }
            catch (JsonException ex)
            {
                throw new DeserializationException(ex.Message);
            }
        }

        public object Deserialize(Type type, string json, bool enforceSecurityProperties = true)
        {
            JsonSerializerSettings serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = enforceSecurityProperties
                    ? new JsonSecurityPropertyContractResolver() {NamingStrategy = _deserializerNamingStrategy}
                    : new DefaultContractResolver() {NamingStrategy = _deserializerNamingStrategy}
            };
            try
            {
                object result = JsonConvert.DeserializeObject(json, type, serializerSettings);
                return result;
            }
            catch (JsonReaderException ex)
            {
                throw new DeserializationException(ex.Path, ex.LineNumber, ex.LinePosition);
            }
            catch (JsonException ex)
            {
                throw new DeserializationException(ex.Message);
            }
        }

        /// <summary>
        /// Serialize the object
        /// </summary>
        /// <param name="value">The value to serialize</param>
        /// <param name="enforceSecurityProperties">True if [SecurityProperty] should be honoured (not serialized)</param>
        /// <typeparam name="TModel">The model type</typeparam>
        /// <returns>The serialized string</returns>
        public string Serialize<TModel>(TModel value, bool enforceSecurityProperties = true)
        {
            JsonSerializerSettings serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = enforceSecurityProperties
                    ? new JsonSecurityPropertyContractResolver() {NamingStrategy = _serializerNamingStrategy}
                    : new DefaultContractResolver() {NamingStrategy = _serializerNamingStrategy},
                Converters = _jsonConverters
            };
            
            return JsonConvert.SerializeObject(value, Formatting.Indented, serializerSettings);
        }
    }
}