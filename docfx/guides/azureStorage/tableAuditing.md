# Auditing with Azure Table Storage

The Azure Storage extension package contains an auditors that supports output to Azure Table Storage. To get started you'll need to install the Azure Storage NuGet package:

    Install-Package AzureFromTheTrenches.Commanding.AzureStorage

And then register the auditing system with your IoC container:

    CloudStorageAccount storageAccount = ...;
    ICommandRegistry registry = resolver.AddCommanding();
    resolver.AddQueues().AddAzureStorageCommandAuditing(storageAccount);

By default this will bind to all audit events (pre-dispatch, post-dispatch and post-execution) and output audit details into two tables:

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
