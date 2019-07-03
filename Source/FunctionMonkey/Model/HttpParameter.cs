using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FunctionMonkey.Abstractions.Extensions;
using Microsoft.AspNetCore.Http;

namespace FunctionMonkey.Model
{
    public class HttpParameter
    {
        public string Name { get; set; }

        public string TypeName => Type.EvaluateType();

        public bool IsString => TypeName.Equals("System.String");

        public Type Type { get; set; }

        public bool IsFormCollection => Type == typeof(IFormCollection);

        public bool IsEnum => Type.IsEnum;

        public bool IsCollection => typeof(IEnumerable<>).IsAssignableFrom(Type);

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

        public string CollectionInstanceTypeName => CollectionInstanceType.EvaluateType();

        public bool IsNullable => Type.IsGenericType && Type.GetGenericTypeDefinition() == typeof(Nullable<>);

	    public string NullableType => Type.GetGenericArguments().First().FullName;

	    public bool IsNullableTypeHasTryParseMethod => IsNullable && Type.GetGenericArguments().First()
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
