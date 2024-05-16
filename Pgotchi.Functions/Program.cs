using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Pgotchi.Functions;
using Pgotchi.Functions.Functions;
using System.Text.Encodings.Web;
using System.Text.Json;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication((builder) =>
    {
        builder.Services.AddOptions<AzureIotHubOptions>()
            .BindConfiguration(AzureIotHubOptions.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        builder.Services.AddOptions<AzureIotHubEventHubOptions>()
            .BindConfiguration(AzureIotHubEventHubOptions.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        builder.Services.Configure<JsonSerializerOptions>(options =>
        {
            options.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
        });

        builder.Services.AddServerlessHub<DeviceTelemetryHub>();
    })
    .Build();

await host.RunAsync();
