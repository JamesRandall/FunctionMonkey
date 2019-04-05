using System.IO;

namespace FunctionMonkey.Abstractions.Builders
{
    public interface IOutputBindingBuilder<out TFunctionTypeBuilder>
    {
        TFunctionTypeBuilder ServiceBusQueue(string connectionStringSettingName, string queueName);
        TFunctionTypeBuilder ServiceBusQueue(string queueName);
        TFunctionTypeBuilder ServiceBusTopic(string connectionStringSettingName, string topicName);
        TFunctionTypeBuilder ServiceBusTopic(string topicName);
        TFunctionTypeBuilder SignalRMessage(string connectionStringSettingName, string hubName);
        TFunctionTypeBuilder SignalRMessage(string hubName);        
        TFunctionTypeBuilder SignalRGroupAction(string connectionStringSettingName, string hubName);
        TFunctionTypeBuilder SignalRGroupAction(string hubName);
        //TFunctionTypeBuilder StorageBlob(string connectionStringSettingName, string name, FileAccess fileAccess = FileAccess.Write); // can use multiples of these 
        //TFunctionTypeBuilder StorageBlob(string name, FileAccess fileAccess = FileAccess.Write); // can use multiples of these 
        TFunctionTypeBuilder StorageQueue(string connectionStringSettingName, string queueName);
        TFunctionTypeBuilder StorageQueue(string queueName);
        TFunctionTypeBuilder StorageTable(string connectionStringSettingName, string tableName);
        TFunctionTypeBuilder StorageTable(string tableName);
        TFunctionTypeBuilder CosmosDb(string connectionStringSettingName, string collectionName, string databaseName);
        TFunctionTypeBuilder CosmosDb(string collectionName, string databaseName);
    }
}