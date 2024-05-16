using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Pgotchi.Functions;
using System.Text.Encodings.Web;
using System.Text.Json;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication(builder =>
    {
        builder.Services.AddOptions<AzureIotHubEventHubOptions>()
        .BindConfiguration(AzureIotHubEventHubOptions.SectionName)
        .ValidateDataAnnotations()
        .ValidateOnStart();

        builder.Services.Configure<JsonSerializerOptions>(options =>
        {
            options.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
        });
    })
    .Build();

await host.RunAsync();
