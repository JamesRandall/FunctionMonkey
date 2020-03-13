using System;
using System.IO;
using System.Linq.Expressions;
using AzureFromTheTrenches.Commanding.Abstractions;

namespace FunctionMonkey.Abstractions.Builders
{
    public interface IOutputBindingBuilder<out TFunctionTypeBuilder>
    {
        TFunctionTypeBuilder ServiceBusQueue(string connectionStringSettingName, string queueName);
        TFunctionTypeBuilder ServiceBusQueue<TResult>(string connectionStringSettingName, string queueName, Expression<Func<TResult,object>> sessionIdProperty=null);
        TFunctionTypeBuilder ServiceBusQueue(string queueName);
        TFunctionTypeBuilder ServiceBusQueue<TResult>(string queueName, Expression<Func<TResult,object>> sessionIdProperty); 
        TFunctionTypeBuilder ServiceBusTopic(string connectionStringSettingName, string topicName);
        TFunctionTypeBuilder ServiceBusTopic<TResult>(string connectionStringSettingName, string topicName, Expression<Func<TResult,object>> sessionIdProperty=null);
        TFunctionTypeBuilder ServiceBusTopic(string topicName);
        TFunctionTypeBuilder ServiceBusTopic<TResult>(string topicName, Expression<Func<TResult,object>> sessionIdProperty=null);
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
        TFunctionTypeBuilder EventHub(string connectionStringSettingName, string hubName);
        TFunctionTypeBuilder EventHub(string hubName);
    }
}