using System;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace FunctionMonkey.Compiler.Core.Implementation.OpenApi
{
    public static class OpenApiXmlCommentsHelper
    {
        private static Regex RefTagPattern = new Regex(@"<(see|paramref) (name|cref)=""([TPF]{1}:)?(?<display>.+?)"" ?/>");
        private static Regex CodeTagPattern = new Regex(@"<c>(?<display>.+?)</c>");
        private static Regex ParaTagPattern = new Regex(@"<para>(?<display>.+?)</para>", RegexOptions.Singleline);

        public static string Humanize(string text)
        {
            if (text == null)
                throw new ArgumentNullException("text");

            return text
                .NormalizeIndentation()
                .HumanizeRefTags()
                .HumanizeCodeTags()
                .HumanizeParaTags();
        }

        private static string NormalizeIndentation(this string text)
        {
            string[] lines = text.Split('\n');
            string padding = GetCommonLeadingWhitespace(lines);

            int padLen = padding == null ? 0 : padding.Length;

            // remove leading padding from each line
            for (int i = 0, l = lines.Length; i < l; ++i)
            {
                string line = lines[i].TrimEnd('\r'); // remove trailing '\r'

                if (padLen != 0 && line.Length >= padLen && line.Substring(0, padLen) == padding)
                    line = line.Substring(padLen);

                lines[i] = line;
            }

            // remove leading empty lines, but not all leading padding
            // remove all trailing whitespace, regardless
            return string.Join("\r\n", lines.SkipWhile(x => string.IsNullOrWhiteSpace(x))).TrimEnd();
        }

        private static string GetCommonLeadingWhitespace(string[] lines)
        {
            if (null == lines)
                throw new ArgumentException("lines");

            if (lines.Length == 0)
                return null;

            string[] nonEmptyLines = lines
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToArray();

            if (nonEmptyLines.Length < 1)
                return null;

            int padLen = 0;

            // use the first line as a seed, and see what is shared over all nonEmptyLines
            string seed = nonEmptyLines[0];
            for (int i = 0, l = seed.Length; i < l; ++i)
            {
                if (!char.IsWhiteSpace(seed, i))
                    break;

                if (nonEmptyLines.Any(line => line[i] != seed[i]))
                    break;

                ++padLen;
            }

            if (padLen > 0)
                return seed.Substring(0, padLen);

            return null;
        }

        private static string HumanizeRefTags(this string text)
        {
            return RefTagPattern.Replace(text, (match) => match.Groups["display"].Value);
        }

        private static string HumanizeCodeTags(this string text)
        {
            return CodeTagPattern.Replace(text, (match) => "{" + match.Groups["display"].Value + "}");
        }

        private static string HumanizeParaTags(this string text)
        {
            return ParaTagPattern.Replace(text, (match) => "<br>" + match.Groups["display"].Value);
        }
        public static string GetMemberNameForMethod(MethodInfo method)
        {
            var builder = new StringBuilder("M:");

            builder.Append(QualifiedNameFor(method.DeclaringType));
            builder.Append($".{method.Name}");

            var parameters = method.GetParameters();
            if (parameters.Any())
            {
                var parametersNames = parameters.Select(p =>
                {
                    return p.ParameterType.IsGenericParameter
                        ? $"`{p.ParameterType.GenericParameterPosition}"
                        : QualifiedNameFor(p.ParameterType, expandGenericArgs: true);
                });
                builder.Append($"({string.Join(",", parametersNames)})");
            }

            return builder.ToString();
        }

        public static string GetMemberNameForType(Type type)
        {
            var builder = new StringBuilder("T:");
            builder.Append(QualifiedNameFor(type));

            return builder.ToString();
        }

        public static string GetNodeNameForMember(MemberInfo memberInfo)
        {
            var builder = new StringBuilder(((memberInfo.MemberType & MemberTypes.Field) != 0) ? "F:" : "P:");
            builder.Append(QualifiedNameFor(memberInfo.DeclaringType));
            builder.Append($".{memberInfo.Name}");

            return builder.ToString();
        }

        private static string QualifiedNameFor(Type type, bool expandGenericArgs = false)
        {
            if (type.IsArray)
                return $"{QualifiedNameFor(type.GetElementType(), expandGenericArgs)}[]";

            var builder = new StringBuilder();

            if (!string.IsNullOrEmpty(type.Namespace))
                builder.Append($"{type.Namespace}.");

            if (type.IsNested)
                builder.Append($"{type.DeclaringType.Name}.");

            if (type.IsConstructedGenericType && expandGenericArgs)
            {
                var nameSansGenericArgs = type.Name.Split('`').First();
                builder.Append(nameSansGenericArgs);

                var genericArgsNames = type.GetGenericArguments().Select(t =>
                {
                    return t.IsGenericParameter
                        ? $"`{t.GenericParameterPosition}"
                        : QualifiedNameFor(t, true);
                });

                builder.Append($"{{{string.Join(",", genericArgsNames)}}}");
            }
            else
            {
                builder.Append(type.Name);
            }

            return builder.ToString();
        }

        public static bool TryCreateFor(OpenApiSchema schema, object value, out IOpenApiAny openApiAny)
        {
            openApiAny = null;

            if (schema.Type == "boolean" && TryCast(value, out bool boolValue))
                openApiAny = new OpenApiBoolean(boolValue);

            else if (schema.Type == "integer" && schema.Format == "int32" && TryCast(value, out int intValue))
                openApiAny = new OpenApiInteger(intValue);

            else if (schema.Type == "integer" && schema.Format == "int64" && TryCast(value, out long longValue))
                openApiAny = new OpenApiLong(longValue);

            else if (schema.Type == "number" && schema.Format == "float" && TryCast(value, out float floatValue))
                openApiAny = new OpenApiFloat(floatValue);

            else if (schema.Type == "number" && schema.Format == "double" && TryCast(value, out double doubleValue))
                openApiAny = new OpenApiDouble(doubleValue);

            else if (schema.Type == "string" && value.GetType().IsEnum)
                openApiAny = new OpenApiString(Enum.GetName(value.GetType(), value));

            else if (schema.Type == "string" && schema.Format == "date-time" && TryCast(value, out DateTime dateTimeValue))
                openApiAny = new OpenApiDate(dateTimeValue);

            else if (schema.Type == "string")
                openApiAny = new OpenApiString(value.ToString());

            return openApiAny != null;
        }

        private static bool TryCast<T>(object value, out T typedValue)
        {
            try
            {
                typedValue = (T)Convert.ChangeType(value, typeof(T));
                return true;
            }
            catch (InvalidCastException)
            {
                typedValue = default(T);
                return false;
            }
        }
    }
}
