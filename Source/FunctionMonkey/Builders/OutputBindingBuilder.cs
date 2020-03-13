using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Abstractions.Builders;
using FunctionMonkey.Abstractions.Builders.Model;
using FunctionMonkey.Abstractions.SignalR;
using FunctionMonkey.Commanding.Abstractions;
using FunctionMonkey.Model.OutputBindings;

namespace FunctionMonkey.Builders
{
    internal class OutputBindingBuilder<TParentBuilder> : IOutputBindingBuilder<TParentBuilder>
    {
        private readonly ConnectionStringSettingNames _connectionStringSettingNames;
        private readonly TParentBuilder _parentBuilder;
        private readonly AbstractFunctionDefinition _functionDefinition;
        private readonly Type _pendingOutputConverterType;

        public OutputBindingBuilder(
            ConnectionStringSettingNames connectionStringSettingNames,
            TParentBuilder parentBuilder,
            AbstractFunctionDefinition functionDefinition,
            Type pendingOutputConverterType
            )
        {
            _connectionStringSettingNames = connectionStringSettingNames;
            _parentBuilder = parentBuilder;
            _functionDefinition = functionDefinition;
            _pendingOutputConverterType = pendingOutputConverterType;
        }
        
        public TParentBuilder ServiceBusQueue(string connectionString, string queueName)
        {
            VerifyOutputBinding();
            
            _functionDefinition.OutputBinding = new ServiceBusQueueOutputBinding(_functionDefinition, connectionString)
            {
                QueueName = queueName,
                OutputBindingConverterType = _pendingOutputConverterType
            };
            
            return _parentBuilder;
        }

        public TParentBuilder ServiceBusQueue(string queueName)
        {
            return ServiceBusQueue(_connectionStringSettingNames.ServiceBus, queueName);
        }
        
        public TParentBuilder ServiceBusQueue<TResult>(string connectionString, string queueName, Expression<Func<TResult,object>> sessionIdProperty)
        {
            VerifyOutputBinding();
            string sessionIdPropertyName = null;
            if (sessionIdProperty != null)
            {
                sessionIdPropertyName = GetMemberName(sessionIdProperty.Body);
            }
            _functionDefinition.OutputBinding = new ServiceBusQueueOutputBinding(_functionDefinition, connectionString)
            {
                QueueName = queueName,
                SessionIdPropertyName = sessionIdPropertyName,
                OutputBindingConverterType = _pendingOutputConverterType
            };

            return _parentBuilder;
        }
        
        public TParentBuilder ServiceBusQueue<TResult>(string queueName, Expression<Func<TResult,object>> sessionIdProperty=null)
        {
            return ServiceBusQueue(_connectionStringSettingNames.ServiceBus, queueName, sessionIdProperty);
        }
        
        public TParentBuilder ServiceBusTopic(string connectionString, string topicName)
        {
            VerifyOutputBinding();
            _functionDefinition.OutputBinding = new ServiceBusTopicOutputBinding(_functionDefinition, connectionString)
            {
                TopicName = topicName,
                OutputBindingConverterType = _pendingOutputConverterType
            };

            return _parentBuilder;
        }
        
        public TParentBuilder ServiceBusTopic(string topicName)
        {
            return ServiceBusTopic(_connectionStringSettingNames.ServiceBus, topicName);
        }

        public TParentBuilder ServiceBusTopic<TResult>(string connectionString, string topicName, Expression<Func<TResult,object>> sessionIdProperty=null)
        {
            VerifyOutputBinding();
            string sessionIdPropertyName = null;
            if (sessionIdProperty != null)
            {
                sessionIdPropertyName = GetMemberName(sessionIdProperty.Body);
            }
            _functionDefinition.OutputBinding = new ServiceBusTopicOutputBinding(_functionDefinition, connectionString)
            {
                TopicName = topicName,
                SessionIdPropertyName = sessionIdPropertyName,
                OutputBindingConverterType = _pendingOutputConverterType
            };

            return _parentBuilder;
        }

        public TParentBuilder ServiceBusTopic<TResult>(string topicName, Expression<Func<TResult,object>> sessionIdProperty)
        {
            return ServiceBusTopic(_connectionStringSettingNames.ServiceBus, topicName, sessionIdProperty);
        }

        public TParentBuilder SignalRMessage(string hubName)
        {
            return SignalRMessage(_connectionStringSettingNames.SignalR, hubName);
        }

        public TParentBuilder SignalRMessage(string connectionStringSettingName, string hubName)
        {
            VerifyOutputBinding();
            _functionDefinition.OutputBinding = new SignalROutputBinding(_functionDefinition, connectionStringSettingName)
            {
                HubName = hubName,
                SignalROutputTypeName = SignalROutputBinding.SignalROutputMessageType, // can't use typeof() here as we don't want to bring the SignalR package into here
                OutputBindingConverterType = _pendingOutputConverterType
            };
            return _parentBuilder;
        }

        public TParentBuilder SignalRGroupAction(string connectionStringSettingName, string hubName)
        {
            VerifyOutputBinding();
            
            _functionDefinition.OutputBinding = new SignalROutputBinding(_functionDefinition,
                connectionStringSettingName)
            {
                HubName = hubName,
                SignalROutputTypeName = SignalROutputBinding.SignalROutputGroupActionType, // can't use typeof() here as we don't want to bring the SignalR package into here
                OutputBindingConverterType = _pendingOutputConverterType
            };
            return _parentBuilder;
        }

        public TParentBuilder SignalRGroupAction(string hubName)
        {
            return SignalRGroupAction(_connectionStringSettingNames.SignalR, hubName);
        }

        public TParentBuilder StorageBlob(string connectionStringSettingName, string name, FileAccess fileAccess = FileAccess.Write)
        {
            if (_functionDefinition.OutputBinding is null)
            {
                _functionDefinition.OutputBinding = new StorageBlobOutputBinding(_functionDefinition);
                _functionDefinition.OutputBinding.OutputBindingConverterType = _pendingOutputConverterType;
            }

            if (_functionDefinition.OutputBinding is StorageBlobOutputBinding blobBinding)
            {
                blobBinding.Outputs.Add(new StorageBlobOutput(_functionDefinition, connectionStringSettingName)
                {
                    FileAccess = fileAccess,
                    Name = name
                });
            }
            else
            {
                throw new ConfigurationException($"An output binding is already set for command {_functionDefinition.CommandType.Name}");
            }

            return _parentBuilder;
        }

        public TParentBuilder StorageBlob(string name, FileAccess fileAccess = FileAccess.Write)
        {
            return StorageBlob(_connectionStringSettingNames.Storage, name, fileAccess);
        }

        public TParentBuilder StorageQueue(string connectionStringSettingName, string queueName)
        {
            VerifyOutputBinding();
            _functionDefinition.OutputBinding = new StorageQueueOutputBinding(_functionDefinition, connectionStringSettingName)
            {
                QueueName = queueName,
                OutputBindingConverterType = _pendingOutputConverterType
            };
            return _parentBuilder;
        }

        public TParentBuilder StorageQueue(string queueName)
        {
            return StorageQueue(_connectionStringSettingNames.Storage, queueName);
        }

        public TParentBuilder StorageTable(string connectionStringSettingName, string tableName)
        {
            VerifyOutputBinding();
            _functionDefinition.OutputBinding = new StorageTableOutputBinding(_functionDefinition, connectionStringSettingName)
            {
                TableName = tableName,
                OutputBindingConverterType = _pendingOutputConverterType
            };
            return _parentBuilder;
        }

        public TParentBuilder StorageTable(string tableName)
        {
            return StorageTable(_connectionStringSettingNames.Storage, tableName);
        }

        public TParentBuilder CosmosDb(string connectionStringSettingName, string collectionName, string databaseName)
        {
            VerifyOutputBinding();

            // we can use the command output type to determine whether or not to use a IAsyncCollector or an out parameter
            // if its based on IEnumerable we do the former, otherwise the latter
            bool isCollection = typeof(IEnumerable).IsAssignableFrom(_functionDefinition.CommandResultType);
            
            _functionDefinition.OutputBinding = new CosmosOutputBinding(_functionDefinition, connectionStringSettingName)
            {
                CollectionName = collectionName,
                DatabaseName = databaseName,
                IsCollection = isCollection,
                OutputBindingConverterType = _pendingOutputConverterType
            };

            return _parentBuilder;
        }

        public TParentBuilder CosmosDb(string collectionName, string databaseName)
        {
            return CosmosDb(_connectionStringSettingNames.CosmosDb, collectionName, databaseName);
        }

        public TParentBuilder EventHub(string connectionStringSettingName, string hubName)
        {
            VerifyOutputBinding();
            _functionDefinition.OutputBinding = new EventHubOutputBinding(_functionDefinition, connectionStringSettingName)
            {
                EventHub = hubName,
                OutputBindingConverterType = _pendingOutputConverterType
            };

            return _parentBuilder;
        }

        public TParentBuilder EventHub(string hubName)
        {
            return EventHub(_connectionStringSettingNames.EventHub, hubName);
        }

        private void VerifyOutputBinding()
        {
            if (_functionDefinition.OutputBinding != null)
            {
                throw new ConfigurationException($"An output binding is already set for command {_functionDefinition.CommandType.Name}");
            }
        }
        
        private static string GetMemberName(Expression expression)
        {
            if (expression is UnaryExpression)
            {
                UnaryExpression unaryExpression = (UnaryExpression) expression;
                return GetMemberName(unaryExpression);
            }

            throw new ArgumentException("Only root properties names are supported for session IDs");
        }

        private static string GetMemberName(UnaryExpression unaryExpression)
        {
            if (unaryExpression.Operand is MethodCallExpression)
            {
                var methodExpression = (MethodCallExpression) unaryExpression.Operand;
                return methodExpression.Method.Name;
            }

            return ((MemberExpression) unaryExpression.Operand).Member.Name;
        }
    }
}