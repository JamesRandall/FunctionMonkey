# Auditing with Azure Storage Queues

The Azure Storage extension package contains an auditor that supports output to Azure Storage Queues. To get started you'll need to install the Azure Storage NuGet package:

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
