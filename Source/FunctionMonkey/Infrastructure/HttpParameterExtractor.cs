using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Abstractions.Extensions;
using FunctionMonkey.Model;
using Microsoft.AspNetCore.Http;

namespace FunctionMonkey.Infrastructure
{
    public class HttpParameterExtractor
    {
        private readonly HttpFunctionDefinition _httpFunctionDefinition;

        public HttpParameterExtractor(HttpFunctionDefinition httpFunctionDefinition)
        {
            this._httpFunctionDefinition = httpFunctionDefinition ?? throw new ArgumentNullException(nameof(httpFunctionDefinition));
        }

        public HttpParameter[] ExtractPossibleQueryParameters()
        {
            //Debug.Assert(_httpFunctionDefinition.RouteParameters != null);

            var properties = _httpFunctionDefinition
                .CommandType
                .GetProperties(BindingFlags.Instance | BindingFlags.Public);

            properties = properties
                .Where(x => x.GetCustomAttribute<SecurityPropertyAttribute>() == null
                    && x.SetMethod != null
                    && (x.PropertyType == typeof(string)
                        || x.PropertyType.GetMethods(BindingFlags.Public | BindingFlags.Static).Any(y => y.Name == "TryParse")
                        || x.PropertyType.IsEnum
                        || HttpParameterExtractor.IsNullableType(x.PropertyType))
                    && _httpFunctionDefinition.RouteParameters.All(y => y.Name != x.Name) // we can't be a query parameter and a route parameter
                ).ToArray();

             return properties.Select(x => CreateHttpParameter(x)).ToArray();
        }

        private HttpParameter CreateHttpParameter(PropertyInfo x, bool? optional = null)
        {
            var isOptional = optional ?? !x.PropertyType.IsValueType || HttpParameterExtractor.IsNullableType(x.PropertyType);
            
            return new HttpParameter
            {
                Name = x.Name,
                Type = x.PropertyType,
                TypeName = x.PropertyType.EvaluateType(),
                IsOptional = isOptional,
                IsNullableType = HttpParameterExtractor.IsNullableType(x.PropertyType),
            };
        }

        public HttpParameter[] ExtractPossibleFormParameters()
        {
            return _httpFunctionDefinition
                 .CommandType
                 .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                 .Where(x => x.GetCustomAttribute<SecurityPropertyAttribute>() == null
                             && x.SetMethod != null
                             && (x.PropertyType == typeof(IFormCollection)))
                 .Select(x => CreateHttpParameter(x))
                 .ToArray();
        }

        public HttpParameter[] ExtractRouteParameters()
        {
            List<HttpParameter> routeParameters = new List<HttpParameter>();
            if (_httpFunctionDefinition.Route == null)
            {
                _httpFunctionDefinition.RouteParameters = routeParameters;
                return routeParameters.ToArray();
            }

            PropertyInfo[] candidateCommandProperties = _httpFunctionDefinition
                .CommandType
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(x => x.GetCustomAttribute<SecurityPropertyAttribute>() == null
                            && x.SetMethod != null
                            && (x.PropertyType == typeof(string)
                                || HttpParameterExtractor.IsNullableType(x.PropertyType)
                                || x.PropertyType.GetMethods(BindingFlags.Public | BindingFlags.Static).Any(y => y.Name == "TryParse"))).ToArray();
            Regex regex = new Regex("{(.*?)}");
            MatchCollection matches = regex.Matches(_httpFunctionDefinition.Route);
            foreach (Match match in matches) //you can loop through your matches like this
            {
                string routeParameter = match.Groups[1].Value;
                bool isOptional = routeParameter.EndsWith("?");
                string[] routeParameterParts = routeParameter.Split(':');
                if (routeParameterParts.Length == 0)
                {
                    throw new ConfigurationException($"Bad route parameter in route {_httpFunctionDefinition.Route} for command type {_httpFunctionDefinition.CommandType.FullName}");
                }

                string routeParameterName = routeParameterParts[0].TrimEnd('?');
                PropertyInfo[] candidateProperties = candidateCommandProperties
                    .Where(p => p.Name.ToLower() == routeParameterName.ToLower()).ToArray();
                PropertyInfo matchedProperty = null;
                if (candidateProperties.Length == 1)
                {
                    matchedProperty = candidateProperties[0];
                }
                else if (candidateProperties.Length > 1)
                {
                    matchedProperty = candidateProperties.SingleOrDefault(x => x.Name == routeParameterName);
                }

                if (matchedProperty == null)
                {
                    throw new ConfigurationException($"Unable to match route parameter {routeParameterName} to property on command type {_httpFunctionDefinition.CommandType}");
                }

                bool isPropertyNullable = !matchedProperty.PropertyType.IsValueType ||
                                          HttpParameterExtractor.IsNullableType(matchedProperty.PropertyType);

                string routeTypeName;
                if (isOptional && matchedProperty.PropertyType.IsValueType &&
                    HttpParameterExtractor.IsNullableType(matchedProperty.PropertyType))
                {
                    routeTypeName = $"{matchedProperty.PropertyType.EvaluateType()}?";
                }
                else
                {
                    routeTypeName = matchedProperty.PropertyType.EvaluateType();
                }

                var param = CreateHttpParameter(matchedProperty, isOptional);
                param.RouteName = routeParameterName;
                param.RouteTypeName = routeTypeName;
                routeParameters.Add(param);
            }

            return routeParameters.ToArray();

            /*string lowerCaseRoute = httpFunctionDefinition1.Route.ToLower();
            httpFunctionDefinition1.RouteParameters = httpFunctionDefinition1
                .CommandType
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(x => x.GetCustomAttribute<SecurityPropertyAttribute>() == null
                            && x.SetMethod != null
                            && (x.PropertyType == typeof(string) || x.PropertyType
                                    .GetMethods(BindingFlags.Public | BindingFlags.Static).Any(y => y.Name == "TryParse"))
                            && lowerCaseRoute.Contains("{" + x.Name.ToLower() + "}"))
                .Select(x => new HttpParameter
                {
                    Name = x.Name,
                    TypeName = x.PropertyType.EvaluateType(),
                    Type = x.PropertyType
                })
                .ToArray();*/
        }

        public static bool IsNullableType(Type t)
        {
            return Nullable.GetUnderlyingType(t) != null;
            //return (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>));
        }
    }
}
