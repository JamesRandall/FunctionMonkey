namespace FunctionMonkey.Model.OutputBindings
{
    internal class CosmosOutputBinding : AbstractConnectionStringOutputBinding
    {
        public bool IsCollection { get; set; }
        
        public string DatabaseName { get; set; }
        
        public string CollectionName { get; set; }
    }
}