namespace AzureFromTheTrenches.Commanding.AzureFunctions.Model
{
    public class HttpQueryParameter
    {
        public string Name { get; set; }

        public string TypeName { get; set; }

        public bool IsString => TypeName.Equals("System.String");
    }
}
