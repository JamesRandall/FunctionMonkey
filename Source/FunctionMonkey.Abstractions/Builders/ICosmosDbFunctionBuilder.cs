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
        /// <returns></returns>
        ICosmosDbFunctionBuilder ChangeFeedFunction<TCommand>(string collectionName, string databaseName, string leaseCollectionName="leases")
            where TCommand : ICommandClaimsBinder;
    }
}
