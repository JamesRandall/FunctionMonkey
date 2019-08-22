namespace FunctionMonkey.FSharp

open FunctionMonkey.Model.OutputBindings
open Models
open FunctionMonkey.Extensions

module OutputBindings =
    
    let cosmosDbWithConnectionStringSettingName databaseName collectionName connectionStringSettingName (outputBindingTarget:IOutputBindingTarget<'functionType>) =
        outputBindingTarget.setOutputBinding(
            new CosmosOutputBinding(
               FunctionMonkey.Extensions.Utils.EvaluateType(outputBindingTarget.resultType),
               connectionStringSettingName,
               DatabaseName = databaseName,
               CollectionName = collectionName
            )
        )
    
    let cosmosDb databaseName collectionName (outputBindingTarget:IOutputBindingTarget<'functionType>) =
        cosmosDbWithConnectionStringSettingName databaseName collectionName "" outputBindingTarget

            

