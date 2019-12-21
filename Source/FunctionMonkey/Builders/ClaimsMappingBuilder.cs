using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Claims;
using FunctionMonkey.Abstractions;
using FunctionMonkey.Abstractions.Builders;
using FunctionMonkey.Infrastructure;
using FunctionMonkey.Model;

namespace FunctionMonkey.Builders
{
    internal class ClaimsMappingBuilder : IClaimsMappingBuilder
    {
        private readonly List<ClaimMappingDefinition> _claimMappingDefinitions = new List<ClaimMappingDefinition>();

        private readonly Dictionary<Type, List<CommandClaimMappingDefinition>> _commandClaimMappingDefinitions = new Dictionary<Type, List<CommandClaimMappingDefinition>>();

        public IClaimsMappingBuilder MapClaimToPropertyName(string claimType, string propertyName)
        {
            _claimMappingDefinitions.Add(new ClaimMappingDefinition
            {
                ClaimType = claimType,
                PropertyName = propertyName
            });
            return this;
        }

        public IClaimsMappingBuilder MapClaimToCommandProperty<TCommand>(string claimType, Expression<Func<TCommand, object>> getProperty)
        {
            if (!_commandClaimMappingDefinitions.TryGetValue(typeof(TCommand), out List<CommandClaimMappingDefinition> list))
            {
                list = new List<CommandClaimMappingDefinition>();
                _commandClaimMappingDefinitions[typeof(TCommand)] = list;
            }

            PropertyInfo propertyInfo;
            if (getProperty.Body is UnaryExpression unaryExpression)
            {
                propertyInfo = (PropertyInfo)((MemberExpression)unaryExpression.Operand).Member;
            }
            else
            {
                propertyInfo = (PropertyInfo)((MemberExpression)getProperty.Body).Member;
            }

            list.Add(new CommandClaimMappingDefinition
            {
                ClaimType = claimType,
                CommandType = typeof(TCommand),
                PropertyInfo = propertyInfo
            });
            return this;
        }

        public ICommandClaimsBinder Build(IReadOnlyCollection<Type> commandTypes)
        {
            Type claimsPrincipalType = typeof(ClaimsPrincipal);
            MethodInfo findFirstClaim = claimsPrincipalType.GetMethod("FindFirst", new[] { typeof(string) });
            Type claimType = typeof(Claim);
            MethodInfo getClaimValue = claimType.GetProperty("Value").GetMethod;
            Dictionary<Type, Func<object, ClaimsPrincipal, object>> compiledClaimMappers = new Dictionary<Type, Func<object, ClaimsPrincipal, object>>();

            foreach (Type commandType in commandTypes)
            {
                IReadOnlyCollection<ClaimMapping> mappings = GetMappingsForCommandType(commandType);

                List<Expression> blocks = new List<Expression>();
                ParameterExpression claimsPrincipalParameter = Expression.Parameter(typeof(ClaimsPrincipal));
                ParameterExpression commandParameter = Expression.Parameter(typeof(object));

                // TODO: These needs to deal with nullable value types like Guid?

                // For each claim mapping this essentially builds out an expression like the C# below for string properties
                //
                //    Claim claim = claimsPrincipal.FindFirst("UserId");
                //    if (claim != null)
                //    {
                //        query.StringProperty = claim.Value;
                //    }
                //
                // And like this for value properties (or in fact any type that has a static method Parse(string)):
                //
                //    Claim claim = claimsPrincipal.FindFirst("UserId");
                //    if (claim != null)
                //    {
                //        query.IntProperty = int.Parse(claim.Value);
                //    }
                foreach (ClaimMapping mapping in mappings)
                {
                    ParameterExpression claimVariable = Expression.Variable(typeof(Claim));

                    Expression claimValueParserExpression;
                    if (mapping.ToPropertyInfo.PropertyType == typeof(string))
                    {
                        claimValueParserExpression = Expression.Call(claimVariable, getClaimValue);
                    }
                    else
                    {
                        MethodInfo parseMethod = mapping.ToPropertyInfo.PropertyType.GetMethod("Parse",
                            BindingFlags.Public | BindingFlags.Static,
                            null,
                            new[] { typeof(string) },
                            null);
                        if (parseMethod == null)
                        {
                            throw new ClaimMappingCompilationException(
                                commandType,
                                mapping.FromClaimType,
                                mapping.ToPropertyInfo.Name,
                                mapping.ToPropertyInfo.PropertyType);
                        }
                        claimValueParserExpression = Expression.Call(parseMethod, Expression.Call(claimVariable, getClaimValue));
                    }

                    Expression block = Expression.Block(
                        new[] { claimVariable },
                        Expression.Assign(claimVariable, Expression.Call(claimsPrincipalParameter, findFirstClaim, Expression.Constant(mapping.FromClaimType))),
                        Expression.IfThen(Expression.NotEqual(claimVariable, Expression.Constant(null)),
                            Expression.Call(
                                Expression.Convert(commandParameter, commandType),
                                mapping.ToPropertyInfo.SetMethod, claimValueParserExpression))
                    );
                    blocks.Add(block);
                }
                //blocks.Add(Expression.Block(typeof(object),  Expression.Constant(commandParameter)));
                blocks.Add(Expression.Block(typeof(object), commandParameter));
                

                var lambda = Expression.Lambda<Func<object, ClaimsPrincipal, object>>(Expression.Block(blocks), commandParameter,
                    claimsPrincipalParameter);
                Func<object, ClaimsPrincipal, object> compiledMapper = lambda.Compile();
                compiledClaimMappers[commandType] = compiledMapper;
            }

            return new CommandClaimsBinder(compiledClaimMappers);
        }

        private IReadOnlyCollection<ClaimMapping> GetMappingsForCommandType(Type commandType)
        {
            Dictionary<string, PropertyInfo> commandProperties = commandType.GetProperties().ToDictionary(x => x.Name, x => x);
            Dictionary<string, ClaimMapping> mappingsByPropertyName = new Dictionary<string, ClaimMapping>();

            // we do the generic non-command specific claim to property mappings first as these are overridden by any command
            // specific mappings
            foreach (ClaimMappingDefinition genericMappingDefinition in _claimMappingDefinitions)
            {
                if (commandProperties.TryGetValue(genericMappingDefinition.PropertyName, out PropertyInfo property))
                {
                    mappingsByPropertyName[property.Name] = new ClaimMapping
                    {
                        FromClaimType = genericMappingDefinition.ClaimType,
                        ToPropertyInfo = property
                    };
                }
            }

            // then we do command specific mappings, these override generic mappings
            if (_commandClaimMappingDefinitions.TryGetValue(commandType, out List<CommandClaimMappingDefinition> commandClaimMappingDefinitions))
            {
                foreach (CommandClaimMappingDefinition definition in commandClaimMappingDefinitions)
                {
                    mappingsByPropertyName[definition.PropertyInfo.Name] = new ClaimMapping
                    {
                        FromClaimType = definition.ClaimType,
                        ToPropertyInfo = definition.PropertyInfo
                    };
                }
            }

            return mappingsByPropertyName.Values;
        }
    }
}
