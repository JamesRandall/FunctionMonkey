using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Claims;
using FunctionMonkey.Abstractions.Http;
using FunctionMonkey.Model;

namespace FunctionMonkey.Infrastructure
{
    internal static class ImmutableCommandClaimsMapperBuilder
    {
        public static Func<object, ClaimsPrincipal, object> Build(
            HttpFunctionDefinition functionDefinition,
            IReadOnlyCollection<AbstractClaimsMappingDefinition> claimsMappings)
        {
            if (functionDefinition.ImmutableTypeConstructorParameters.Count == 0)
            {
                return (o, cp) => o;
            }

            IReadOnlyCollection<CommandPropertyClaimsMappingDefinition> commandMappings = claimsMappings
                .Where(t => t is CommandPropertyClaimsMappingDefinition).Cast<CommandPropertyClaimsMappingDefinition>()
                .ToList();
            IReadOnlyCollection<SharedClaimsMappingDefinition> sharedMappings = claimsMappings
                .Where(t => t is SharedClaimsMappingDefinition).Cast<SharedClaimsMappingDefinition>()
                .ToList();

            bool didBuildAMappingFunc = false;
            List<Expression> constructorClaims = new List<Expression>();
            
            ParameterExpression claimsPrincipalParameter = Expression.Parameter(typeof(ClaimsPrincipal));
            ParameterExpression commandParameter = Expression.Parameter(typeof(object));
            
            foreach (ImmutableTypeConstructorParameter constructorParameter in functionDefinition
                .ImmutableTypeConstructorParameters)
            {
                // there could be a shared mapping and a command -> property mapping - we use the latter first
                // in order of precedence
                CommandPropertyClaimsMappingDefinition commandPropertyDefinition = commandMappings.SingleOrDefault(
                    x => x.CommandType == functionDefinition.CommandType &&
                         x.PropertyInfo.Name == constructorParameter.Name);
                if (commandPropertyDefinition != null)
                {
                    constructorClaims.Add(BuildExpressionForPropertyName(
                        commandPropertyDefinition.ClaimName,
                        constructorParameter.Type,
                        claimsPrincipalParameter,
                        functionDefinition.CommandType, 
                        constructorParameter.Name, 
                        commandParameter)
                    );
                    didBuildAMappingFunc = true;
                }
                else
                {
                    SharedClaimsMappingDefinition sharedDefinition = sharedMappings.SingleOrDefault(
                        x => x.PropertyPath == constructorParameter.Name);
                    if (sharedDefinition != null)
                    {
                        constructorClaims.Add(BuildExpressionForPropertyName(
                            sharedDefinition.ClaimName,
                            constructorParameter.Type,
                            claimsPrincipalParameter,
                            functionDefinition.CommandType, 
                            constructorParameter.Name, 
                            commandParameter));
                        didBuildAMappingFunc = true;
                    }
                    else
                    {
                        constructorClaims.Add(BuildExpressionForPreviousValue(functionDefinition.CommandType, constructorParameter.Name, commandParameter));
                    }
                }
            }

            if (!didBuildAMappingFunc)
            {
                return (o, cp) => o;
            }

            ConstructorInfo constructorInfo = functionDefinition.CommandType.GetConstructor(
                functionDefinition.ImmutableTypeConstructorParameters.Select(x => x.Type).ToArray());
            return BuildConstructorFunc(constructorInfo, constructorClaims, commandParameter, claimsPrincipalParameter);
        }

        private static Expression BuildExpressionForPropertyName(
            string claimName,
            Type constructorParameterType,
            ParameterExpression claimsPrincipalParameter,
            Type commandType, string constructorParameterName, ParameterExpression commandParameter)
        {
            Type claimsPrincipalType = typeof(ClaimsPrincipal);
            MethodInfo findFirstClaim = claimsPrincipalType.GetMethod("FindFirst", new[] { typeof(string) });
            Type claimType = typeof(Claim);
            MethodInfo getClaimValue = claimType.GetProperty("Value").GetMethod;

            Expression getClaim =
                Expression.Call(claimsPrincipalParameter, findFirstClaim, Expression.Constant(claimName));
            Expression claimValueParserExpression;
            if (constructorParameterType == typeof(string))
            {
                //claimValueParserExpression = Expression.Call(getClaim, getClaimValue);
                claimValueParserExpression = Expression.Condition(
                    Expression.Equal(getClaim, Expression.Constant(null, typeof(Claim))),
                    BuildExpressionForPreviousValue(commandType, constructorParameterName, commandParameter),
                    Expression.Call(getClaim, getClaimValue)
                );
            }
            else
            {
                MethodInfo parseMethod = constructorParameterType.GetMethod("Parse",
                    BindingFlags.Public | BindingFlags.Static,
                    null,
                    new[] { typeof(string) },
                    null);
                if (parseMethod == null)
                {
                    throw new ConfigurationException("Claims must be mapped to strings or data types that have a Parse method");
                }
                
                claimValueParserExpression = Expression.Condition(
                    Expression.Equal(getClaim, Expression.Constant(null)),
                    BuildExpressionForPreviousValue(commandType, constructorParameterName, commandParameter),
                    Expression.Call(parseMethod, Expression.Call(getClaim, getClaimValue))
                    );
                
                //claimValueParserExpression = Expression.Call(parseMethod, Expression.Call(getClaim, getClaimValue));
            }

            return claimValueParserExpression;
        }
        
        private static Expression BuildExpressionForPreviousValue(Type commandType, string constructorParameterName, ParameterExpression commandParameter)
        {
            MethodInfo getter = commandType.GetProperty(constructorParameterName)?.GetMethod;
            if (getter == null)
            {
                throw new ConfigurationException("Property cannot be found that matches the constructor parameter.");
            }

            return Expression.Call(Expression.Convert(commandParameter, commandType), getter);
        }

        private static Func<object, ClaimsPrincipal, object> BuildConstructorFunc(
            ConstructorInfo constructorInfo,
            IReadOnlyCollection<Expression> parameters,
            ParameterExpression commandParameter,
            ParameterExpression claimsPrincipalParameter)
        {
            return Expression.Lambda<Func<object, ClaimsPrincipal, object>>(
                Expression.Convert(Expression.New(constructorInfo, parameters), typeof(object)),
                    commandParameter,
                    claimsPrincipalParameter).Compile();
        }
    }
}