using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FunctionMonkey.Abstractions.Extensions;
using FunctionMonkey.Extensions;
using Microsoft.AspNetCore.Http;

namespace FunctionMonkey.Model
{
    public class HttpParameter
    {
        public string Name { get; set; }

        public string TypeName => Type.EvaluateType();

        public bool IsString => DiscreteTypeName.Equals("System.String");

        public Type Type { get; set; }

        public bool IsFormCollection => Type == typeof(IFormCollection);

        public bool IsEnum => DiscreteType.IsEnum;

        public bool IsCollection => Type.IsSupportedQueryParameterCollectionType();

        public bool IsCollectionArray => Type.IsArray;

        public Type CollectionInstanceType
        {
	        get
	        {
		        if (Type.IsInterface)
		        {
			        // we need to figure out the right collection type for the interface
			        Type genericList = typeof(List<>);
			        Type typedList = genericList.MakeGenericType(Type.GenericTypeArguments[0]);
			        return typedList;
		        }

		        // TODO: Get generic argument from IEnumerable and make sure the collection
		        // has an add method
		        return Type; // can we always return the type if its a concrete collection?
	        }
        }

        public Type DiscreteType =>
	        (Type.IsSupportedQueryParameterCollectionType() ? Type.SupportedCollectionValueType() : Type);

        public string DiscreteTypeName => DiscreteType.EvaluateType();

        public string CollectionInstanceTypeName => CollectionInstanceType.EvaluateType();

        public bool IsNullable => DiscreteType.IsGenericType && DiscreteType.GetGenericTypeDefinition() == typeof(Nullable<>);

	    public string NullableType => DiscreteType.GetGenericArguments().First().FullName;

	    public bool IsNullableTypeHasTryParseMethod => IsNullable && DiscreteType.GetGenericArguments().First()
		                                                   .GetMethods(BindingFlags.Public | BindingFlags.Static)
		                                                   .Any(x => x.Name == "TryParse");

        public bool IsOptional { get; set; }

        // The below applies to route parameters
        public string RouteName { get; set; }
        public string RouteTypeName { get; set; }
        public bool IsNullableType { get; set; }
        public bool IsGuid => Type == typeof(Guid) || Type == typeof(Guid?);
    }
}
