# Auditing and Event Sourcing

A benefit of invoking actions through commands expressed as state is that it allows for these actions to be persisted to maintain a log or audit of all the interactions that took place. Given an appropriate data storage and command handler design this further allows the persisted events to be utilised with patterns such as [event sourcing](https://martinfowler.com/eaaDev/EventSourcing.html) or compensating transactions in order to deal with failed operations concerned with, for example, distributed none-transactional storage operations.

This makes the command pattern with a mediator particularly useful for distributed systems running in the cloud where such requirements are common.

The framework has extension points for auditors at three points in the command dispatch and execution process:

1. Before the framework dispatches the command
2. After the framework has successfully dispatched the command but before it has executed the command
3. After the framework has executed the command (this also logs whether this completed successfully or encountered errors)

Auditors are required to implement the ICommandAuditor interface and are constructed by the commanding framework the first time they are used (and then held onto) through the IoC container and so can be injected with any registered dependencies. For example:

    public class ConsoleAuditor : ICommandAuditor
    {
        public Task Audit(AuditItem item, CancellationToken cancellationToken)
        {
            ConsoleColor previousColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine($"Type: {item.CommandTypeFullName}");
            Console.WriteLine($"Correlation ID: {item.CorrelationId}");
            Console.WriteLine($"Depth: {item.Depth}");
            foreach (KeyValuePair<string, string> enrichedProperty in item.AdditionalProperties)
            {
                Console.WriteLine($"{enrichedProperty.Key}: {enrichedProperty.Value}");
            }
            Console.ForegroundColor = previousColor;
            return Task.FromResult(0);
        }        
    }

Auditors must then be registered with the commanding system before any commands are dispatched:

    ICommandingDependencyResolverAdapter resolver = ...;
    ICommandRegistry registry = resolver.AddCommanding();
    // register commands here
    resolver.AddPreDispatchCommandingAuditor<ConsoleAuditor>();
    resolver.AddPostDispatchCommandingAuditor<ConsoleAuditor>();
    resolver.AddExecutionDispatchCommandingAuditor<ConsoleAuditor>();

By default auditors will only audit the root command i.e. if one command handler dispatches a command from within itself only the initial command will be audited. To change this behavior and audit all commands pass false to the registration methods, for example:

    resolver.AddPreDispatchCommandingAuditor<ConsoleAuditor>(false);

Finally it's worth noting that multiple auditors can be configured for each event so it's possible to write the command to an event store and also log some basic details into a diagnostic log system.

## Presupplied Auditors

The framework is supplied with a number of prebuilt auditors and these are described below.

### Azure Event Hubs

By adding this package commands will be audited to an Azure event hub. First install the NuGet package:

    Install-Package AzureFromTheTrenches.Commanding.AzureEventHub

Then register the auditor:

    resolver.AddEventHubCommandAuditing("eventhubconnectionstring", "myhub");

For high volume systems a partition key provider can be supplied by implementing and supplying an instance of an IPartitionKeyProvider. And finally a parameter can be supplied of type AzureEventHubAuditorOptions that determines what audit events the auditor is bound to and whether each audit should only audit the root command (this option set defaults to all events and root command only). For example:

    resolver.AddEventHubCommandAuditing("eventhubconnectionstring", "myhub", new MyPartitionKeyProvider(), new AzureEventHubAuditorOptions {
        UsePostDispatchAuditor = false
    });

### Azure Table Storage

By adding this package commands will be audited to Azure Table Storage. First install the NuGet package:

    Install-Package AzureFromTheTrenches.Commanding.AzureStorage

Then register the auditor:

    CloudStorageAccount account = ...;
    resolver.AddAzureStorageCommandAuditing(cloudStorageAccount);

By default this will bind to all audit events and output audit details into two tables:

    commandauditbydate
    commandauditbycorrelationid

It's possible to also output the full command payload into blob storage by passing a blob container reference when configuring storage:

    CloudStorageAccount account = ...;
    CloudBlobContainer container = ...;
    resolver.AddAzureStorageCommandAuditing(cloudStorageAccount, container);

Alternative storage strategies can be provided by supplying an implementation of the IStorageStrategy interface (the auditor comes with two strategies built in: SingleTableStrategy and TablePerDayStrategy). And finally a parameter can be supplied of type AzureStorageAuditorOptions that determines what audit events the auditor is bound to and whether each audit should only audit the root command (this option set defaults to all events and root command only). For example:

    CloudStorageAccount account = ...;
    CloudBlobContainer container = ...;
    resolver.AddAzureStorageCommandAuditing(cloudStorageAccount, container, new TablePerDayStrategy(), new AzureStorageAuditorOptions  {
        UsePostDispatchAuditor = false
    });

### Azure Storage Queues

By adding this package commands will be audited to an Azure Storage Queue. First install the NuGet package:

    Install-Package AzureFromTheTrenches.Commanding.AzureStorage

Then register the auditor:

    CloudQueue queue = ...;
    resolver.AddAzureStorageCommandAuditing(queue);

A blob container to persist full command payloads and auditor options can also be supplied:

    CloudQueue queue = ...;
    CloudBlobContainer container = ...;
    resolver.AddAzureStorageCommandAuditing(queue, container, null, new AzureStorageAuditorOptions  {
        UsePostDispatchAuditor = false
    });

_(Note: the third, null, parameter is for an unused storage strategy and will be removed in a future version)_

Events can be popped from the queue and further processed by use of the built in queue processor - this will read the queue and write the events to configured auditors. For example if you want to write audit information out to multiple storage providers you may not want to do this inline in your services execution due to the added latency this would cost. Using the queue and a queue processor along with the standard audit mechanisms allows the audit to take place asynchronously.

An example of this is [available here](https://github.com/JamesRandall/AccidentalFish.Commanding/tree/master/Samples/AzureStorageAuditing).

### Microsoft Logging Extensions

A summary of audit events can be sent to the Microsoft.Extensions.Logging framework. To install this auditor first add the package:

    Install-Package AzureFromTheTrenches.Commanding.MicrosoftLoggingExtensions

And then register the auditors:

    resolver.AddMicrosoftLoggingExtensionsAuditor();

By default dispatch operations and successful executions will be logged at the Trace level and execution failures at the Warning level (this default may change to Error in a future version depending on feedback) however these can be overridden:

    resolver.AddMicrosoftLoggingExtensionsAuditor(
        LogLevel.Information, // the level for dispatch and successful execution
        LogLevel.Error // the level for failed execution
    );

Finally a parameter can be supplied of type MicrosoftLoggingExtensionsAuditorOptions that determines what audit events the auditor is bound to and whether each audit should only audit the root command (this option set defaults to all events and root command only). For example:

    resolver.AddMicrosoftLoggingExtensionsAuditor(
        LogLevel.Information, // the level for dispatch and successful execution
        LogLevel.Error, // the level for failed execution
        new MicrosoftLoggingExtensionsAuditorOptions {
            UseExecutionAuditor=false,
            AuditPreDispatchRootOnly=false
        }
    );