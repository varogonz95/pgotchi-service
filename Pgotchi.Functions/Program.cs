using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
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

        builder.Use(next =>
            async context =>
            {
                var requestData = await context.GetHttpRequestDataAsync();
                var namingPolicy = Enum.Parse<ResponsePropertyNamingPolicy>(requestData?.Query["namingPolicy"] ?? string.Empty, true);
                var jsonSerializerOptions = context.InstanceServices.GetRequiredService<IOptions<JsonSerializerOptions>>().Value;

                jsonSerializerOptions.PropertyNamingPolicy = ResolveJsonNamingPolicy(namingPolicy);

                await next(context);
            });
    })
    .Build();

await host.RunAsync();

static JsonNamingPolicy ResolveJsonNamingPolicy(ResponsePropertyNamingPolicy namingPolicy) =>
    namingPolicy switch
    {
        ResponsePropertyNamingPolicy.SnakeCaseUpper => JsonNamingPolicy.SnakeCaseUpper,
        ResponsePropertyNamingPolicy.SnakeCaseLower => JsonNamingPolicy.SnakeCaseLower,
        ResponsePropertyNamingPolicy.KebabCaseUpper => JsonNamingPolicy.KebabCaseUpper,
        ResponsePropertyNamingPolicy.KebabCaseLower => JsonNamingPolicy.KebabCaseLower,
        ResponsePropertyNamingPolicy.CapitalCase => new JsonCapitalCaseNamingPolicy(),
        _ => JsonNamingPolicy.CamelCase,
    };
