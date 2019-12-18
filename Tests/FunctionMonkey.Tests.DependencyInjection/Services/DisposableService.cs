namespace FunctionMonkey.Tests.DependencyInjection.Services
{
	using System.Security.Claims;
	using Microsoft.Extensions.Logging;

	internal class DisposableService : IDisposableService
	{
		private readonly ILogger _logger;

		public DisposableService(ILogger<DisposableService> logger, ClaimsPrincipal principal, ICallContext context)
		{
			_logger = logger;

			_logger.LogDebug($"DisposableService created for {principal?.FindFirst("name")?.Value}.");
			_logger.LogWarning($"{context.Subject}, {context.Name}");
		}

		public void Dispose()
		{
			_logger.LogDebug("DisposableService disposed.");
		}

		public void Run()
		{
			_logger.LogInformation("LogInformation.");
			_logger.LogWarning("LogWarning.");
			_logger.LogError("LogError.");
		}
	}
}
