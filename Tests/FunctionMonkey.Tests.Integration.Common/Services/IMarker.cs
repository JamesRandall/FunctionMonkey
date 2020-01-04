using System;
using System.Threading.Tasks;

namespace FunctionMonkey.Tests.Integration.Common.Services
{
    public interface IMarker
    {
        Task RecordMarker(Guid markerId);
    }
}