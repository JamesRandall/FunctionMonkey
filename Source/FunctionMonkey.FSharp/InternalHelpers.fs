namespace FunctionMonkey.FSharp
open FunctionMonkey.Abstractions
open System
open System.Linq.Expressions
open System.Reflection
open Models
open FunctionMonkey.Abstractions.Extensions

module internal InternalHelpers =
    
    let private fsharpOptionType = typedefof<Option<_>>
    let isFSharpOptionType (tp:Type) = tp.IsGenericType && (tp.GetGenericTypeDefinition() = fsharpOptionType)
    
    let optionTypeInnerName (tp:Type) = if (isFSharpOptionType tp) then tp.GetGenericArguments().[0].EvaluateType() else ""
                                        
    let optionTypeInnerTypeIsString (tp:Type) = if (isFSharpOptionType tp) then tp.GetGenericArguments().[0] = typeof<string> else false
    
    let gatherModuleFunctions (assembly:Assembly) =
        assembly.GetTypes()
        |> Seq.collect (
               fun t -> t.GetProperties(BindingFlags.Public + BindingFlags.Static)
                        |> Seq.filter(fun p  -> p.PropertyType = typeof<Functions>)
           )
        |> Seq.map (fun p ->
            let propertyValue = p.GetValue(null)
            propertyValue :?> Functions)
        |> Seq.toList
        
    let concatFunctions functionsListA functionsListB =
        {
            httpFunctions = functionsListA |> Seq.collect(fun f -> f.httpFunctions)
                            |> Seq.append (functionsListB |> Seq.collect(fun f -> f.httpFunctions))
                            |> Seq.toList
            serviceBusFunctions = functionsListA |> Seq.collect(fun f -> f.serviceBusFunctions)
                            |> Seq.append (functionsListB |> Seq.collect(fun f -> f.serviceBusFunctions))
                            |> Seq.toList
            timerFunctions = functionsListA |> Seq.collect(fun f -> f.timerFunctions)
                            |> Seq.append (functionsListB |> Seq.collect(fun f -> f.timerFunctions))
                            |> Seq.toList
            storageFunctions = functionsListA |> Seq.collect(fun f -> f.storageFunctions)
                            |> Seq.append (functionsListB |> Seq.collect(fun f -> f.storageFunctions))
                            |> Seq.toList
            cosmosDbFunctions = functionsListA |> Seq.collect(fun f -> f.cosmosDbFunctions)
                            |> Seq.append (functionsListB |> Seq.collect(fun f -> f.cosmosDbFunctions))
                            |> Seq.toList
        }
        
    let getPropertyInfo (expression:Expression<Func<'commandType, 'propertyType>>) =
        let memberExpression =
            if expression.Body :? UnaryExpression then
                let unaryExpression = expression.Body :?> UnaryExpression
                unaryExpression.Operand :?> MemberExpression
            else
                expression.Body :?> MemberExpression
            
        memberExpression.Member :?> PropertyInfo
        
    let gatherBacklinkPropertyInfo (assembly:Assembly) =
        let functionCompilerMetadataProperty =
            assembly.GetTypes()
            |> Seq.collect (
               fun t -> t.GetProperties(BindingFlags.Public + BindingFlags.Static)
                        |> Seq.filter(fun p  -> typeof<IFunctionCompilerMetadata>.IsAssignableFrom(p.PropertyType))
               )
            |> Seq.toList

        // we have to find at least one of these as the presence of an IFunctionnCompilerMetadata is what triggers this
        functionCompilerMetadataProperty.[0]
                               
        

