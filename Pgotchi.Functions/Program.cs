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
                var jsonSerializerOptions = context.InstanceServices.GetRequiredService<IOptions<JsonSerializerOptions>>().Value;
                var namingPolicyQuery = requestData?.Query["namingPolicy"] ?? string.Empty;

                if (Enum.TryParse<ResponsePropertyNamingPolicy>(namingPolicyQuery, out var namingPolicy))
                {
                    jsonSerializerOptions.PropertyNamingPolicy = ResolveJsonNamingPolicy(namingPolicy);
                }
                else
                {
                    jsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                }

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
