namespace FunctionMonkey.Tests.DependencyInjection
{
	using System.IO;
	using System.Threading.Tasks;

	using FunctionMonkey.Tests.DependencyInjection.Services;

	using Microsoft.AspNetCore.Http;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.Azure.WebJobs;
	using Microsoft.Azure.WebJobs.Extensions.Http;
	using Microsoft.Extensions.Logging;

	using Newtonsoft.Json;

	public class Function1
	{
		private readonly IDisposableService _service;

		public Function1(IDisposableService service)
		{
			_service = service;
		}

		public static string Props { get; set; }

		public string Prop { get; set; }

		[FunctionName("Function1")]
		public async Task<IActionResult> Run(
			[HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
			ILogger log)
		{
			log.LogInformation("C# HTTP trigger function processed a request.");

			string name = req.Query["name"];

			string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
			dynamic data = JsonConvert.DeserializeObject(requestBody);
			name = name ?? data?.name;

			_service.Run();

			return name != null
				? (ActionResult)new OkObjectResult($"Hello, {name}")
				: new BadRequestObjectResult("Please pass a name on the query string or in the request body");
		}
	}
}
