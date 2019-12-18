using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace FunctionMonkey.Testing.Implementation
{
    internal class AspNetRuntime
    {
        public IServiceProvider ServiceProvider { get; }

        public AspNetRuntime()
        {
            // TODO: Need to resolve namespace
            /*
            IWebHost host =
                Microsoft.AspNetCore.WebHost.CreateDefaultBuilder()
                    .ConfigureServices(sc => { sc.AddMvc(); })
                    .UseStartup<DummyStartup>()
                    .Build();
            ServiceProvider = host.Services;*/
        }

        public async Task<HttpResponse> CreateHttpResponse(ActionContext actionContext, IActionResult actionResult)
        {
            await actionResult.ExecuteResultAsync(actionContext);
            return SanitizeResponse(actionContext.HttpContext.Response);
        }

        public ActionContext PrepareToExecuteHttp(ICommand command,
            HttpFunctionDefinition httpFunctionDefinition,
            HttpMethod method)
        {            

            HttpContext httpContext = new DefaultHttpContext()
            {
                RequestServices = ServiceProvider,
                Request =
                {
                    Method = (method ?? httpFunctionDefinition.Verbs.Single()).Method
                },
                Response = { Body = new MemoryStream() }
            };
            RouteData routeData = new RouteData();
            ActionDescriptor actionDescriptor = new ActionDescriptor();
            return new ActionContext(httpContext, routeData, actionDescriptor);
        }

        private HttpResponse SanitizeResponse(HttpResponse httpContextResponse)
        {
            httpContextResponse.Body.Seek(0, SeekOrigin.Begin);
            if (httpContextResponse.ContentLength == null)
            {
                httpContextResponse.ContentLength = 0;
            }
            return httpContextResponse;
        }

        private class DummyStartup : IStartup
        {
            public IServiceProvider ConfigureServices(IServiceCollection services)
            {
                return services.BuildServiceProvider();
            }

            public void Configure(IApplicationBuilder app)
            {

            }
        }        
    }
}
