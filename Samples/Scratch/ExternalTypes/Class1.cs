using System;
using IndirectExternalTypes;

namespace ExternalTypes
{
    public class Message
    {
        public string Text { get; set; }

        public SomeType Some { get; set; } = new SomeType();
    }
}
