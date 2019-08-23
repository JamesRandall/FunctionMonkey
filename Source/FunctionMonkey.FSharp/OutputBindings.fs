namespace FunctionMonkey.FSharp

open FunctionMonkey.Model.OutputBindings
open Models
open FunctionMonkey.Extensions

module OutputBindings =
    
    let cosmosDbWithConnectionStringSettingName collectionName databaseName  connectionStringSettingName (outputBindingTarget:IOutputBindingTarget<'functionType>) =
        outputBindingTarget.setOutputBinding(
            new CosmosOutputBinding(
               FunctionMonkey.Extensions.Utils.EvaluateType(outputBindingTarget.resultType),
               connectionStringSettingName,
               DatabaseName = databaseName,
               CollectionName = collectionName
            )
        )
    
    let cosmosDb collectionName databaseName  (outputBindingTarget:IOutputBindingTarget<'functionType>) =
        cosmosDbWithConnectionStringSettingName collectionName databaseName  "" outputBindingTarget

            

