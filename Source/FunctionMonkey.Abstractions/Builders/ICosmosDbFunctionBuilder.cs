using AzureFromTheTrenches.Commanding.Abstractions;

namespace FunctionMonkey.Abstractions.Builders
{
    public interface ICosmosDbFunctionBuilder
    {
        /// <summary>
        /// Associate a function with the specified collection and database
        /// </summary>
        /// <typeparam name="TCommand"></typeparam>
        /// <param name="collectionName">The Cosmos collection name</param>
        /// <param name="databaseName">The Cosmos database name</param>
        /// <param name="leaseCollectionName">The cosmos collection to use for leases - defaults to leases</param>
        /// <param name="leaseDatabaseName">The cosmos database to use for leases - defaults to the database name</param>
        /// <param name="createLeaseCollectionIfNotExists">Creates the lease collection if it doesn't exist, defaults to false</param>
        /// <param name="startFromBeginning">Start from the beginning of the change feed, defaults to false</param>
        /// <param name="convertToPascalCase">
        /// Is the Cosmos document in camel case. Defaults to true in which case C# property names will be converted to camel case. Note that
        /// due to limitations in the current Functions / Cosmos Change Feed Processor this involves serializing to a memory stream and then
        /// deserializing.
        ///
        /// If this limitation continues then I will seek to optimize this by compiling camel case model classes and mapping them to pascal
        /// case C# classes via Roslyn.
        ///
        /// Alternatively you can use the JsonProperty attribute on all your properties. Messy I know.
        ///
        /// Due to the potential expense this defaults to false.
        /// </param>
        /// <returns></returns>
        ICosmosDbFunctionBuilder ChangeFeedFunction<TCommand>(string collectionName,
            string databaseName,
            string leaseCollectionName="leases",
            string leaseDatabaseName=null,
            bool createLeaseCollectionIfNotExists=false,
            bool startFromBeginning=false,
            bool convertToPascalCase=false)
            where TCommand : ICommand;
    }
}
