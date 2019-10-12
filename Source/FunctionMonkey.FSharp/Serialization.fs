module FunctionMonkey.FSharp.Serialization
open FunctionMonkey.Serialization
open Microsoft.FSharp.Reflection
open Newtonsoft.Json
open System

// Converters thanks to https://gist.github.com/eulerfx
type private OptionConverter() =
    inherit JsonConverter()
    
    override x.CanConvert(t) = 
        t.IsGenericType && t.GetGenericTypeDefinition() = typedefof<option<_>>

    override x.WriteJson(writer, value, serializer) =
        let value = 
            if value = null then null
            else 
                let _,fields = FSharpValue.GetUnionFields(value, value.GetType())
                fields.[0]  
        serializer.Serialize(writer, value)

    override x.ReadJson(reader, t, existingValue, serializer) =        
        let innerType = t.GetGenericArguments().[0]
        let innerType = 
            if innerType.IsValueType then (typedefof<Nullable<_>>).MakeGenericType([|innerType|])
            else innerType        
        let value = serializer.Deserialize(reader, innerType)
        let cases = FSharpType.GetUnionCases(t)
        if value = null then FSharpValue.MakeUnion(cases.[0], [||])
        else FSharpValue.MakeUnion(cases.[1], [|value|])

let private converters = [
    OptionConverter() :> JsonConverter
]

let private serializer =
    NamingStrategyJsonSerializer(
        Newtonsoft.Json.Serialization.CamelCaseNamingStrategy(),
        converters
    ) 

let modelSerializer (model:obj) (encodeSecurityProperties:bool) : string =
    let output = serializer.Serialize(model, encodeSecurityProperties)
    output
