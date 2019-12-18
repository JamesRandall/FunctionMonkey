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
        
        public bool IsFSharpList { get; set; }

        public bool IsEnum => DiscreteType.IsEnum;

        public bool IsCollection => Type.IsSupportedCSharpQueryParameterCollectionType() || IsFSharpList;

        public bool IsCollectionArray => Type.IsArray;
        
        public bool HasHeaderMapping { get; set; }

        public bool IsTryParse => !IsEnum && !IsCollection && !IsCollectionArray &&
                                  Type.GetMembers().Any(x => x.Name == "TryParse");

        // We currently look to see if we are dealing with an F# discriminated union type by looking for an assignment
        // pattern of a type having a constructor with one parameter that is of the same type of a readonly Item property
        public bool IsDiscriminatedUnion => !IsTryParse &&
                                            Type.GetProperty("Item") != null &&
                                            !Type.GetProperty("Item").CanWrite &&
                                            Type.GetConstructor(new [] { Type.GetProperty("Item").PropertyType }) != null;

        public Type DiscriminatedUnionUnderlyingType => Type.GetProperty("Item")?.PropertyType;

        public string DiscriminatedUnionUnderlyingTypeName => DiscriminatedUnionUnderlyingType?.EvaluateType();

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
	        (IsCollection ? Type.SupportedCollectionValueType() : Type);

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
        
        public bool IsFSharpOptionType { get; set; }
        
        public string FSharpOptionInnerTypeName { get; set; }
        
        public bool FSharpOptionInnerTypeIsString { get; set; }
    }
}
