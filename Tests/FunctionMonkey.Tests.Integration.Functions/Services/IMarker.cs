using System;
using System.Threading.Tasks;

namespace FunctionMonkey.Tests.Integration.Functions.Services
{
    internal interface IMarker
    {
        Task RecordMarker(Guid markerId);
    }
}