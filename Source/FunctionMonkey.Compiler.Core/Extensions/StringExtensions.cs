// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------
// comes from https://github.com/Microsoft/OpenAPI.NET.CSharpAnnotations

using System;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace FunctionMonkey.Compiler.Core.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="string"/>.
    /// </summary>
    internal static class StringExtensions
    {
        private static readonly Regex AllNonCompliantCharactersRegex = new Regex(@"[^a-zA-Z0-9\.\-_]");
        private static readonly Regex GenericMarkersRegex = new Regex(@"`[0-9]+");

        /// <summary>
        /// Determines whether the string contains the given substring using the specified StringComparison.
        /// </summary>
        /// <param name="value">The full string.</param>
        /// <param name="substring">The substring to check.</param>
        /// <param name="stringComparison">Stirng comparison.</param>
        /// <returns>Whether the string contains the given substring using the specified StringComparison.</returns>
        public static bool Contains(this string value, string substring, StringComparison stringComparison)
        {
            return value.IndexOf(substring, stringComparison) >= 0;
        }

        /// <summary>
        /// Gets the type name from the "cref" value.
        /// </summary>
        /// <param name="value">The cref value.</param>
        /// <returns>The type name.</returns>
        public static string ExtractTypeNameFromCref(this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }

            return Regex.IsMatch(value, "^T:") ? value.Split(':')[1] : value;
        }

        /// <summary>
        /// Removes blank lines.
        /// </summary>
        /// <param name="value">The original string.</param>
        /// <returns>The updated string.</returns>
        public static string RemoveBlankLines(this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }

            return Regex.Replace(
                    value,
                    @"^\s*$(\r\n|\r|\n)",
                    string.Empty,
                    RegexOptions.Multiline)
                .Trim();
        }

        /// <summary>
        /// Sanitizes class name to satisfy the OpenAPI V3 restriction, i.e.
        /// match the regular expression ^[a-zA-Z0-9\.\-_]+$.
        /// </summary>
        /// <param name="value">The original class name string.</param>
        /// <returns>The sanitized class name.</returns>
        public static string SanitizeClassName(this string value)
        {
            // Replace + (used when this type has a parent class name) by .
            value = value.Replace(oldChar: '+', newChar: '.');

            // Remove `n from a generic type. It's clear that this is a generic type
            // since it will be followed by other types name(s).
            value = GenericMarkersRegex.Replace(value, string.Empty);

            // Replace , (used to separate multiple types used in a generic) by - 
            value = value.Replace(oldChar: ',', newChar: '-');

            // Replace all other non-compliant strings, including [ ] used in generics by _
            value = AllNonCompliantCharactersRegex.Replace(value, "_");

            return value;
        }

        /// <summary>
        /// Converts the first letter of the string to uppercase.
        /// </summary>
        /// <param name="value">The original string.</param>
        /// <returns>The updated string.</returns>
        public static string ToTitleCase(this string value)
        {
            if (value == null)
            {
                return null;
            }

            value = value.Trim();

            if (string.IsNullOrWhiteSpace(value))
            {
                return value;
            }

            return value.Substring(startIndex: 0, length: 1).ToUpperInvariant() + value.Substring(startIndex: 1);
        }

        /// <summary>
        /// Extracts the absolute path from a full URL string.
        /// </summary>
        /// <param name="value">The string in URL format.</param>
        /// <returns>The absolute path inside the URL.</returns>
        public static string UrlStringToAbsolutePath(this string value)
        {
            return WebUtility.UrlDecode(new Uri(value).AbsolutePath);
        }

        // Matches JSON.Net implementation
        public static string ToCamelCase(this string s)
        {
            if (string.IsNullOrEmpty(s) || !char.IsUpper(s[0]))
            {
                return s;
            }

            char[] chars = s.ToCharArray();

            for (int i = 0; i < chars.Length; i++)
            {
                if (i == 1 && !char.IsUpper(chars[i]))
                {
                    break;
                }

                bool hasNext = (i + 1 < chars.Length);
                if (i > 0 && hasNext && !char.IsUpper(chars[i + 1]))
                {
                    // if the next character is a space, which is not considered uppercase 
                    // (otherwise we wouldn't be here...)
                    // we want to ensure that the following:
                    // 'FOO bar' is rewritten as 'foo bar', and not as 'foO bar'
                    // The code was written in such a way that the first word in uppercase
                    // ends when if finds an uppercase letter followed by a lowercase letter.
                    // now a ' ' (space, (char)32) is considered not upper
                    // but in that case we still want our current character to become lowercase
                    if (char.IsSeparator(chars[i + 1]))
                    {
                        chars[i] = ToLower(chars[i]);
                    }

                    break;
                }

                chars[i] = ToLower(chars[i]);
            }

            return new string(chars);
        }

        private static char ToLower(char c)
        {
#if HAVE_CHAR_TO_STRING_WITH_CULTURE
            c = char.ToLower(c, CultureInfo.InvariantCulture);
#else
            c = char.ToLowerInvariant(c);
#endif
            return c;
        }

        public static string FriendlyId(this Type type, bool fullyQualified = false)
        {
            var str = fullyQualified ? type.FullNameSansTypeArguments().Replace("+", ".") : type.Name;
            if (!type.GetTypeInfo().IsGenericType)
            {
                return str;
            }
            var array = type.GetGenericArguments().Select(t => t.FriendlyId(fullyQualified)).ToArray();
            return new StringBuilder(str).Replace($"`{array.Count()}", string.Empty).Append(
                $"[{string.Join(",", array).TrimEnd(',')}]").ToString();
        }

        private static string FullNameSansTypeArguments(this Type type)
        {
            if (string.IsNullOrEmpty(type.FullName))
            {
                return string.Empty;
            }
            var fullName = type.FullName;
            var length = fullName.IndexOf("[[", StringComparison.Ordinal);
            return length != -1 ? fullName.Substring(0, length) : fullName;
        }
    }
}