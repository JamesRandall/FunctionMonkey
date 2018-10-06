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
        /// <param name="documentIsCamelCase">Is the Cosmos document in camel case. Defaults to true in which case C# property names will be converted to camel case </param>
        /// <returns></returns>
        ICosmosDbFunctionBuilder ChangeFeedFunction<TCommand>(string collectionName,
            string databaseName,
            string leaseCollectionName="leases",
            string leaseDatabaseName=null,
            bool createLeaseCollectionIfNotExists=false,
            bool documentIsCamelCase=true)
            where TCommand : ICommand;
    }
}
