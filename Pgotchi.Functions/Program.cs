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

        builder.Services.Configure<JsonSerializerOptions>(options =>
        {
            options.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
        });
    })
    .Build();

await host.RunAsync();
