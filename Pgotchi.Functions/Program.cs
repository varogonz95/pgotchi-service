using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Pgotchi.Functions.Functions;
using Pgotchi.Shared;
using Pgotchi.Shared.Options;
using System.Text.Encodings.Web;
using System.Text.Json;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication((builder) =>
    {
        var services = builder.Services;

        services.AddOptions<AzureIotHubOptions>()
            .BindConfiguration(AzureIotHubOptions.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddOptions<AzureIotHubEventHubOptions>()
            .BindConfiguration(AzureIotHubEventHubOptions.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.Configure<JsonSerializerOptions>(options =>
        {
            options.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
        });

        services.AddServerlessHub<DeviceTelemetryHub>();

        services.AddAutoMapper(config =>
        {
            config.AddProfile<DataProfile>();
        });
    })
    .Build();

await host.RunAsync();
