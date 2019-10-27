// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// ------------------------------------------------------------
// modified from https://github.com/Microsoft/OpenAPI.NET.CSharpAnnotations

using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Abstractions.OpenApi;
using FunctionMonkey.Compiler.Extensions;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace FunctionMonkey.Compiler.Implementation
{
    internal class SchemaReferenceRegistry
    {
        /// <summary>
        /// The dictionary containing all references of the given type.
        /// </summary>
        private readonly Dictionary<string, OpenApiSchema> _references = new Dictionary<string, OpenApiSchema>();

        private readonly IList<IOpenApiSchemaFilter> _schemaFilters;

        public SchemaReferenceRegistry(IList<IOpenApiSchemaFilter> schemaFilter)
        {
            _schemaFilters = schemaFilter;
        }

        public OpenApiSchema FindReference(Type input)
        {
            // Return empty schema when the type does not have a name. 
            // This can occur, for example, when a generic type without the generic argument specified
            // is passed in.
            if (input == null || input.FullName == null)
            {
                return new OpenApiSchema();
            }

            var key = GetKey(input);

            // If the schema already exists in the References, simply return.
            if (_references.ContainsKey(key))
            {
                return new OpenApiSchema
                {
                    Reference = new OpenApiReference
                    {
                        Id = key,
                        Type = ReferenceType.Schema
                    }
                };
            }

            return null;
        }

        /// <summary>
        /// Finds the existing reference object based on the key from the input or creates a new one.
        /// </summary>
        /// <returns>The existing or created reference object.</returns>
        public OpenApiSchema FindOrAddReference(Type input)
        {
            // Return empty schema when the type does not have a name. 
            // This can occur, for example, when a generic type without the generic argument specified
            // is passed in.
            if (input == null || input.FullName == null)
            {
                return new OpenApiSchema();
            }

            var key = GetKey(input);

            // If the schema already exists in the References, simply return.
            if (_references.ContainsKey(key))
            {
                return new OpenApiSchema
                {
                    Reference = new OpenApiReference
                    {
                        Id = key,
                        Type = ReferenceType.Schema
                    }
                };
            }

            try
            {
                // There are multiple cases for input types that should be handled differently to match the OpenAPI spec.
                //
                // 1. Simple Type
                // 2. Enum Type
                // 3. Dictionary Type
                // 4. Enumerable Type
                // 5. Object Type
                var schema = new OpenApiSchema();

                // Filter schema
                var schemaFilterContext = new OpenApiSchemaFilterContext
                {
                    Type = input
                };

                if (input.IsSimple())
                {
                    schema = input.MapToOpenApiSchema();

                    // Certain simple types yield more specific information.
                    if (input == typeof(char))
                    {
                        schema.MinLength = 1;
                        schema.MaxLength = 1;
                    }
                    else if (input == typeof(Guid))
                    {
                        schema.Example = new OpenApiString(Guid.Empty.ToString());
                    }

                    FilterSchema(schema, _schemaFilters, schemaFilterContext);
                    return schema;
                }

                if (input.IsEnum)
                {
                    schema.Type = "string";
                    foreach (var name in Enum.GetNames(input))
                    {
                        schema.Enum.Add(new OpenApiString(name));
                    }

                    FilterSchema(schema, _schemaFilters, schemaFilterContext);
                    return schema;
                }

                if (input.IsDictionary())
                {
                    schema.Type = "object";
                    schema.AdditionalProperties = FindOrAddReference(input.GetGenericArguments()[1]);

                    FilterSchema(schema, _schemaFilters, schemaFilterContext);
                    return schema;
                }

                if (input.IsEnumerable())
                {
                    schema.Type = "array";

                    schema.Items = FindOrAddReference(input.GetEnumerableItemType());

                    FilterSchema(schema, _schemaFilters, schemaFilterContext);
                    return schema;
                }

                schema.Type = "object";

                // Note this assignment is necessary to allow self-referencing type to finish
                // without causing stack overflow.
                // We can also assume that the schema is an object type at this point.
                _references[key] = schema;

                schemaFilterContext.PropertyNames = new Dictionary<string, string>();
                foreach (var propertyInfo in input.GetProperties())
                {
                    // Ignore Property ?
                    var ignoreProperty = propertyInfo.GetAttributeValue((JsonIgnoreAttribute attribute) => attribute) != null;
                    if (!ignoreProperty)
                    {
                        ignoreProperty = propertyInfo.GetAttributeValue((SecurityPropertyAttribute attribute) => attribute) != null;
                    }
                    if (ignoreProperty)
                    {
                        continue;
                    }

                    // Property Name
                    var propertyName = propertyInfo.GetAttributeValue((JsonPropertyAttribute attribute) => attribute.PropertyName);
                    if (string.IsNullOrWhiteSpace(propertyName))
                    {
                        propertyName = propertyInfo.GetAttributeValue((DataMemberAttribute attribute) => attribute.Name);
                    }
                    if (string.IsNullOrWhiteSpace(propertyName))
                    {
                        propertyName = propertyInfo.Name.ToCamelCase();
                    }

                    // Property Required
                    var propertyRequired = propertyInfo.GetAttributeValue((JsonPropertyAttribute attribute) => attribute.Required) == Required.Always;
                    if (!propertyRequired)
                    {
                        propertyRequired = propertyInfo.GetAttributeValue((RequiredAttribute attribute) => attribute) != null;
                    }
                    if (propertyRequired)
                    {
                        schema.Required.Add(propertyName);
                    }

                    // Min and max length
                    int? minLength = propertyInfo.GetAttributeValue((MinLengthAttribute attribute) => attribute)?.Length;
                    if (minLength == null)
                    {
                        minLength = propertyInfo.GetAttributeValue((StringLengthAttribute attribute) => attribute)?.MinimumLength;
                    }
                    int? maxLength = propertyInfo.GetAttributeValue((MaxLengthAttribute attribute) => attribute)?.Length;
                    if (maxLength == null)
                    {
                        maxLength = propertyInfo.GetAttributeValue((StringLengthAttribute attribute) => attribute)?.MaximumLength;
                    }

                    // Regex pattern
                    var pattern = propertyInfo.GetAttributeValue((RegularExpressionAttribute attribute) => attribute.Pattern);

                    // Inner Schema
                    var innerSchema = FindOrAddReference(propertyInfo.PropertyType);
                    innerSchema.ReadOnly = !propertyInfo.CanWrite;
                    innerSchema.MinLength = minLength;
                    innerSchema.MaxLength = maxLength;
                    innerSchema.Pattern = pattern;
                    schema.Properties[propertyName] = innerSchema;
                    schemaFilterContext.PropertyNames[propertyName] = propertyInfo.Name;
                }
                FilterSchema(schema, _schemaFilters, schemaFilterContext);

                _references[key] = schema;

                return new OpenApiSchema
                {
                    Reference = new OpenApiReference
                    {
                        Id = key,
                        Type = ReferenceType.Schema
                    }
                };
            }
            catch (Exception)
            {
                // Something went wrong while fetching schema, so remove the key if exists from the references.
                if (_references.ContainsKey(key))
                {
                    _references.Remove(key);
                }

                throw;
                //throw new AddingSchemaReferenceFailedException(key, e.Message);
            }
        }

        private void FilterSchema(OpenApiSchema schema, IList<IOpenApiSchemaFilter> schemaFilters, OpenApiSchemaFilterContext schemaFilterContext)
        {
            foreach (var schemaFilter in _schemaFilters)
            {
                schemaFilter.Apply(schema, schemaFilterContext);
            }
        }

        public Dictionary<string, OpenApiSchema> References => _references;

        /// <summary>
        /// Gets the key from the input object to use as reference string.
        /// </summary>
        /// <remarks>
        /// This must match the regular expression ^[a-zA-Z0-9\.\-_]+$ due to OpenAPI V3 spec.
        /// </remarks>
        private string GetKey(Type input)
        {
            var typeName = input.GetAttributeValue((DataContractAttribute attribute) => attribute.Name);
            if (typeName == null)
            {
                // Type.ToString() returns full name for non-generic types and
                // returns a full name without unnecessary assembly information for generic types.
                typeName = input.ToString();
            }

            return typeName.SanitizeClassName();
        }
    }

}
