---
title: Function Monkey
documentType: index
---
<style type="text/css">
footer{
  position: relative;
}
</style>

<div class="hero">
  <div class="wrap">
    <div class="text">
      <strong>Function Monkey</strong>
    </div>    
    <div class="minitext">
    Write testable more elegant Azure Functions with less boilerplate, more consistency, and support for REST APIs.
    </div>
    <div class="buttons-unit">
      <a href="guides/gettingStarted.html" class="button"><i class="fas fa-paper-plane"></i><span style="margin-left: 4px;">Getting Started</span></a>
      <a href="https://github.com/JamesRandall/FunctionMonkey" class="button"><i class="fab fa-github"></i><span style="margin-left: 4px;">View on GitHub</span></a>
    </div>
  </div>
</div>
<div class="key-section">
  <div class="container">
    <div class="row">
      <div class="col-md-10 col-md-offset-1 text-center">
        <section>
          <h2>Elegant Fluent API</h2>
          <p class="lead">
          Use a clean fluent API to create functions with all the boilerplate taken care of and runtime support for authorization and dependency injection.
          </p>
        <pre class="text-left">
<code class="hljs kotlin">public class FunctionAppConfiguration : IFunctionAppConfiguration
{
    public void Build(IFunctionHostBuilder builder)
    {
        builder
            .Setup((serviceCollection, commandRegistry) =>
            {
                serviceCollection
                    .AddLogging()
                    .AddNotificationServices(commandRegistry)
                    .AddExpensesService(commandRegistry)
                    .AddInvoiceServices(commandRegistry);
            })
            .Authorization(authorization => authorization.TokenValidator<IBearerTokenValidator>())
            .AddFluentValidation()
            .Functions(functions => functions
                .HttpRoute("v1/Invoice", route => route
                    .HttpFunction<ListInvoicesQuery>(AuthorizationTypeEnum.TokenValidation, HttpMethod.Get)
                )
                .ServiceBus("serviceBusConnection", serviceBus => serviceBus
                    .SubscriptionFunction<SendEmailCommand>("emaildispatchtopic", "emaildispatchsubscription"))
                .Storage("storageConnectionString", storage => storage
                    .BlobFunction<AttachUploadedExpenseReciept>("expenses/{name}"))
            );
    }
}</code></pre>
                </section>
      </div>
    </div>
  </div>
</div>
<div class="counter-key-section">
  <div class="container">
    <div class="row">
      <div class="col-md-10 col-md-offset-1 text-center">
        <div class="icon-container"><i class="fas fa-code"></i></div>
        <section>
          <h2>Less Boilerplate, Less To Go Wrong</h2>
          <p class="lead">Eliminate the tedious repetitive boilerplate leading to less code, fewer defects, improved consistency, better separation of concerns and more flexibility.</p>
        </section>
      </div>
    </div>
  </div>
</div>
<div class="key-section">
  <div class="container content">
    <div class="row">
      <div class="col-md-10 col-md-offset-1 text-center">
        <div class="icon-container"><i class="fas fa-wrench"></i></div>
        <section>
          <h2>Extensible</h2>
          <p class="lead">Take advantage of the mediator pattern and underlying <a href="https://commanding.azurefromthetrenches.com">mediation framework</a> to keep your codebase clean, super-DRY, and loosely coupled.</p>
        </section>
      </div>
    </div>
  </div>
</div>
