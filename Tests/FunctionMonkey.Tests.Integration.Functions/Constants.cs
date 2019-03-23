namespace FunctionMonkey.Tests.Integration.Functions
{
    internal static class Constants
    {
        public static class Storage
        {
            public static class Table
            {
                public const string Markers = "markers";
            }

            public static class Queue
            {
                public const string TestQueue = "testqueue";
            }

            public static class Blob
            {
                public const string BlobCommandContainer = "blobcommands";

                public const string StreamBlobCommandContainer = "streamblobcommands";

                public const string OutputBlobContainer = "outputblobcontainer";
            }
        }

        public static class ServiceBus
        {
            public const string MarkerQueue = "markerqueue";

            public const string Queue = "testqueue";

            public const string TopicName = "testtopic";

            public const string SubscriptionName = "testsub";
        }

        public static class Cosmos
        {
            public const string Database = "testdatabase";

            public const string Collection = "testcollection";
        }
    }
}
