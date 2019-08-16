namespace FunctionMonkey.FSharp
open System
open System.Linq.Expressions
open System.Reflection
open Models

module internal Helpers =
    let gatherModuleFunctions (assembly:Assembly) =
        assembly.GetTypes()
        |> Seq.collect (
               fun t -> t.GetProperties(BindingFlags.Public + BindingFlags.Static)
                        |> Seq.filter(fun p  -> p.PropertyType = typedefof<Functions>)
           )
        |> Seq.map (fun p -> p.GetValue(null) :?> Functions)
        
    let concatFunctions functionsListA functionsListB =
        {
            httpFunctions = functionsListA |> Seq.collect(fun f -> f.httpFunctions)
                            |> Seq.append (functionsListB |> Seq.collect(fun f -> f.httpFunctions))
                            |> Seq.toList
            serviceBusFunctions = functionsListA |> Seq.collect(fun f -> f.serviceBusFunctions)
                            |> Seq.append (functionsListB |> Seq.collect(fun f -> f.serviceBusFunctions))
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

