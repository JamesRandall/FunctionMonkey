using System;
using System.IO;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Abstractions;
using FunctionMonkey.Tests.Integration.Common.Commands;
using FunctionMonkey.Tests.Integration.Common.Extensions;

namespace FunctionMonkey.Tests.Integration.Common.Handlers
{
    internal class BlobCommandHandler : ICommandHandler<BlobCommand>
    {
        private readonly IContextProvider _contextProvider;

        public BlobCommandHandler(IContextProvider contextProvider)
        {
            _contextProvider = contextProvider;
        }

        public async Task ExecuteAsync(BlobCommand command)
        {
            string filename = Path.GetFileNameWithoutExtension(_contextProvider.BlobContext.Uri.AbsoluteUri);
            Guid filenameMarker = string.IsNullOrWhiteSpace(filename) ? Guid.Empty : Guid.Parse(filename);

            if (filenameMarker == command.MarkerId)
            {
                await command.MarkerId.RecordMarker();
            }
        }
    }
}
