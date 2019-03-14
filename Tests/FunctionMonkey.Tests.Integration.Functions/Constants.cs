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
            }
        }
    }
}
