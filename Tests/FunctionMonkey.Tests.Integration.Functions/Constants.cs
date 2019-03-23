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

                public const string MarkerQueue = "markerqueue";
            }

            public static class Blob
            {
                public const string BlobCommandContainer = "blobcommands";

                public const string StreamBlobCommandContainer = "streamblobcommands";

                public const string OutputBlobContainer = "outputblobcontainer";

                public const string BlobOutputCommandContainer = "outputbindingcontainer";
            }
        }

        public static class ServiceBus
        {
            public const string MarkerQueue = "markerqueue";

            public const string MarkerTopic = "markertopic";

            public const string MarkerSubscription = "markersub";

            public const string Queue = "testqueue";

            public const string TopicName = "testtopic";

            public const string SubscriptionName = "testsub";

            public const string TableOutputQueue = "tableoutput";
        }

        public static class Cosmos
        {
            public const string Database = "testdatabase";

            public const string Collection = "testcollection";

            public const string OutputTableCollection = "outputtablecollection";

            public const string OutputTableLeases = "outputtableleases";
        }
    }
}
