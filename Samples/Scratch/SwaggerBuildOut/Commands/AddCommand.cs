using AzureFromTheTrenches.Commanding.Abstractions;

namespace SwaggerBuildOut.Commands
{
    public class AddCommand : ICommand<int>
    {
        [SecurityProperty]
        public int ValueOne { get; set; }

        public int ValueTwo { get; set; }
    }
}
