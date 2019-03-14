using System;
using FunctionMonkey.Tests.Integration.Common;
using Microsoft.WindowsAzure.Storage;

namespace FunctionMonkey.Tests.Integration.Storage.Helpers
{
    public abstract class AbstractStorageFunctionTest : AbstractIntegrationTest
    {
        public CloudStorageAccount StorageAccount => CloudStorageAccount.Parse(AbstractIntegrationTest.Settings.StorageConnectionString);
    }
}
