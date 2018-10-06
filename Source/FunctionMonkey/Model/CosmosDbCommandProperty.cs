using System;

namespace FunctionMonkey.Model
{
    public class CosmosDbCommandProperty
    {
        public string Name { get; set; }

        public string CosmosPropertyName { get; set; }

        public string TypeName { get; set; }

        public Type Type { get; set; }
    }
}
