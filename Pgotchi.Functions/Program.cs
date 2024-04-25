using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Pgotchi.Functions;
using System.Text.Json;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication(builder =>
    {
        builder.Services.AddOptions<AzureIotHubOptions>()
        .BindConfiguration(AzureIotHubOptions.SectionName)
        .ValidateDataAnnotations()
        .ValidateOnStart();

        //builder.UseMiddleware((context, next) =>
        //{
        //    var httpContext = context.GetHttpContext();
        //    var namingStrategy = httpContext.Request.Query["namingStrategy"];
        //    var scope = context.InstanceServices.CreateScope();

        //    return next();
        //});
    })
    .Build();

await host.RunAsync();
