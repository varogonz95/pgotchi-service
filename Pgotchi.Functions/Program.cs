using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Pgotchi.Functions;
using System.Text.Json;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureFunctionsWebApplication(builder =>
    {
        builder.Services.AddOptions<AzureIotHubOptions>()
        .BindConfiguration(AzureIotHubOptions.SectionName)
        .ValidateDataAnnotations()
        .ValidateOnStart();
    })
    .ConfigureServices(services =>
    {
        services.Configure<JsonSerializerOptions>(config =>
        {
            config.PropertyNameCaseInsensitive = true;
            config.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        });
    })
    .Build();

await host.RunAsync();
