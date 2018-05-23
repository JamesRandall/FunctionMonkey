# Packages

The framework is made up of a number of NuGet packages focused on four areas:

|Area|Description|
|----|-----------|
|ASP.Net Core|Extensions for building REST APIs from commands using a configuration based approach|
|Core|The core packages - these are always required when working with the commanding framework though the non-abstract package will only need to be referenced by the host process|
|Dispatch, Processing and Auditing|These packages deal with auditing, the dispatch to queues, and execution based on pulling from queues|
|Performance|Handle the caching of responses to commands in either in-memory caches or a Redis cache based on command signatures|

The dependencies and organisation of these packages is shown in the diagram below:

<img src="/images/packageArchitecture.png">

And the packages contain the following capabilities:

<table class="table table-bordered table-striped table-condensed">
    <thead>
        <tr>
            <th width="33%">Package</th>
            <th width="67%">Capabilities</th>
        </tr>
    </thead>
    <tbody>
        <tr>
            <td>AzureFromTheTrenches.Commanding</td>
            <td>
                <ul style="padding-left:14px;">
                    <li>Commanding runtime - this normally only needs adding in the host (e.g. in a Web API)</li>
                </ul>
            </td>
        </tr>
        <tr>
            <td>AzureFromTheTrenches.Commanding.Abstractions</td>
            <td>
                <ul style="padding-left:14px;">
                    <li>Contains abstractions for the core commanding framework</li>
                </ul>
            </td>
        </tr>
        <tr>
            <td>AzureFromTheTrenches.Commanding.AspNetCore</td>
            <td>
                <ul style="padding-left:14px;">
                    <li>Configuration based REST commanding</li>
                </ul>
            </td>
        </tr>
        <tr>
            <td>AzureFromTheTrenches.Commanding.AspNetCore.Swashbuckle</td>
            <td>
                <ul style="padding-left:14px;">
                    <li>Configuration based REST commanding Swagger support</li>
                </ul>
            </td>
        </tr>
        <tr>
            <td>AzureFromTheTrenches.Commanding.AzureEventHub</td>
            <td>
                <ul style="padding-left:14px;">
                    <li>Dispatch to Event Hub</li>
                    <li>Audit to Event Hub</li>
                </ul>
            </td>
        </tr>
        <tr>
            <td>AzureFromTheTrenches.Commanding.AzureServiceBus</td>
            <td>
                <ul style="padding-left:14px;">
                    <li>Dispatch to Queue and Topic</li>
                    <li>Execution from Queue and Subscription</li>
                </ul>
            </td>
        </tr>
        <tr>
            <td>AzureFromTheTrenches.Commanding.AzureStorage</td>
            <td>
                <ul style="padding-left:14px;">
                    <li>Dispatch to Queue</li>
                    <li>Execution from Queue</li>
                    <li>Audit to Queue</li>
                    <li>Audit to Table Storage</li>
                </ul>
            </td>
        </tr>
        <tr>
            <td>AzureFromTheTrenches.Commanding.Cache</td>
            <td>
                <ul style="padding-left:14px;">
                    <li>Base package for cache support</li>
                </ul>
            </td>
        </tr>
        <tr>
            <td>AzureFromTheTrenches.Commanding.Cache.MemoryCache</td>
            <td>
                <em>NOTE: This package will be renamed in the future to better reflect its purpose</em>
                <ul style="padding-left:14px;">
                    <li>Caching via the Microsoft.Extensions.Cache framework</li>
                </ul>
            </td>
        </tr>
        <tr>
            <td>AzureFromTheTrenches.Commanding.Cache.Redis</td>
            <td>
                <ul style="padding-left:14px;">
                    <li>Caching with Redis</li>
                </ul>
            </td>
        </tr>
        <tr>
            <td>AzureFromTheTrenches.Commanding.Cache.Redis</td>
            <td>
                <ul style="padding-left:14px;">
                    <li>Caching with Redisk</li>
                </ul>
            </td>
        </tr>
        <tr>
            <td>AzureFromTheTrenches.Commanding.Http</td>
            <td>
                <ul style="padding-left:14px;">
                    <li>Dispatch via HTTP (e.g. to a REST service)</li>
                </ul>
            </td>
        </tr>
        <tr>
            <td>AzureFromTheTrenches.Commanding.Queue</td>
            <td>
                <em>Does not normally require adding directly</em>
                <ul style="padding-left:14px;">
                    <li>Base package for queue dispatch</li>
                </ul>
            </td>
        </tr>
    </tbody>
</table>
