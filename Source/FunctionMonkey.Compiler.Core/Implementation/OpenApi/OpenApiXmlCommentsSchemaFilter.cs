using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Xml.XPath;
using FunctionMonkey.Abstractions.Builders;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace FunctionMonkey.Compiler.Core.Implementation.OpenApi
{
    class OpenApiXmlCommentsSchemaFilter : IOpenApiSchemaFilter
    {
        private const string NodeXPath = "/doc/members/member[@name='{0}']";
        private const string SummaryTag = "summary";
        private const string ExampleXPath = "example";

        private readonly XPathNavigator _xmlNavigator;

        public OpenApiXmlCommentsSchemaFilter(XPathDocument xmlDoc)
        {
            _xmlNavigator = xmlDoc.CreateNavigator();
        }

        public void Apply(OpenApiSchema schema, IOpenApiSchemaFilterContext context)
        {
            TryApplyTypeComments(schema, context.Type);

            if (context.PropertyNames == null)
            {
                return;
            }

            foreach (var keyValuePair in context.PropertyNames)
            {
                if (!schema.Properties.TryGetValue(keyValuePair.Key, out OpenApiSchema propertySchema))
                {
                    continue;
                }

                if (propertySchema.Reference != null)
                {
                    // can't add descriptions to a reference schema
                    continue;
                }

                if (keyValuePair.Value == null)
                {
                    continue;
                }

                var memberInfo = context.Type.GetMember(keyValuePair.Value).FirstOrDefault();
                TryApplyMemberComments(propertySchema, memberInfo);
            }
        }

        private void TryApplyTypeComments(OpenApiSchema schema, Type type)
        {
            var typeNodeName = OpenApiXmlCommentsHelper.GetMemberNameForType(type);
            var typeNode = _xmlNavigator.SelectSingleNode(string.Format(NodeXPath, typeNodeName));

            if (typeNode != null)
            {
                var summaryNode = typeNode.SelectSingleNode(SummaryTag);
                if (summaryNode != null)
                {
                    schema.Description = OpenApiXmlCommentsHelper.Humanize(summaryNode.InnerXml);
                }
            }
        }

        private void TryApplyMemberComments(OpenApiSchema schema, MemberInfo memberInfo)
        {
            var memberNodeName = OpenApiXmlCommentsHelper.GetNodeNameForMember(memberInfo);
            var memberNode = _xmlNavigator.SelectSingleNode(string.Format(NodeXPath, memberNodeName));
            if (memberNode == null) return;

            var summaryNode = memberNode.SelectSingleNode(SummaryTag);
            if (summaryNode != null)
            {
                schema.Description = OpenApiXmlCommentsHelper.Humanize(summaryNode.InnerXml);
            }

            var exampleNode = memberNode.SelectSingleNode(ExampleXPath);
            if (exampleNode != null)
            {
                var exampleString = OpenApiXmlCommentsHelper.Humanize(exampleNode.InnerXml);
                var memberType = (memberInfo.MemberType & MemberTypes.Field) != 0 ? ((FieldInfo)memberInfo).FieldType : ((PropertyInfo)memberInfo).PropertyType;
                schema.Example = ConvertToOpenApiType(memberType, schema, exampleString);
            }
        }

        private static IOpenApiAny ConvertToOpenApiType(Type memberType, OpenApiSchema schema, string stringValue)
        {
            object typedValue;

            try
            {
                typedValue = TypeDescriptor.GetConverter(memberType).ConvertFrom(stringValue);
            }
            catch (Exception)
            {
                return null;
            }

            return OpenApiXmlCommentsHelper.TryCreateFor(schema, typedValue, out IOpenApiAny openApiAny)
                ? openApiAny
                : null;
        }
    }
}
