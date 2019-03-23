using System.IO;

namespace FunctionMonkey.Abstractions.Builders
{
    public interface IOutputBindingBuilder<out TFunctionTypeBuilder>
    {
        TFunctionTypeBuilder ServiceBusQueue(string connectionStringSettingName, string queueName);
        TFunctionTypeBuilder ServiceBusTopic(string connectionStringSettingName, string topicName);
        TFunctionTypeBuilder SignalRMessage(string hubName);
        TFunctionTypeBuilder SignalRMessage(string connectionStringSettingName, string hubName);
        TFunctionTypeBuilder SignalRGroupAdd(string connectionStringSettingName, string hubName);
        TFunctionTypeBuilder SignalRGroupRemove(string connectionStringSettingName, string hubName);
        TFunctionTypeBuilder StorageBlob(string connectionStringSettingName, string name, FileAccess fileAccess = FileAccess.Write); // can use multiples of these 
        TFunctionTypeBuilder StorageQueue(string connectionStringSettingName, string queueName);
        TFunctionTypeBuilder StorageTable(string connectionStringSettingName, string tableName);
        TFunctionTypeBuilder Cosmos(string connectionStringSettingName, string databaseName, string collectionName);
    }
}