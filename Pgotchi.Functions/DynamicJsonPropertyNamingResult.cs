using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using System.Net;
using System.Text.Json;

namespace Pgotchi.Functions;

public class DynamicJsonPropertyNamingResult(object value, HttpStatusCode statusCode = HttpStatusCode.OK) : IActionResult
{
    private readonly object _value = value;
    private readonly HttpStatusCode _statusCode = statusCode;
    private readonly JsonSerializerOptions _options = JsonSerializerOptions.Default;

    public DynamicJsonPropertyNamingResult(object value, JsonSerializerOptions options, HttpStatusCode statusCode = HttpStatusCode.OK) : this(value, statusCode)
    {
        _options = options;
    }

    public async Task ExecuteResultAsync(ActionContext context)
    {
        var propertyNamingQuery = context.HttpContext.Request.Query
            .Where(keyValue => keyValue.Key.Equals("propertyNaming", StringComparison.OrdinalIgnoreCase))
            .Select(kv => kv.Value)
            .FirstOrDefault(StringValues.Empty);
        var couldParsePropertyNamingQueryParam = Enum.TryParse<ResponsePropertyNamingPolicy>(propertyNamingQuery, true, out var namingPolicy);

        if (!couldParsePropertyNamingQueryParam)
        {
            throw new Exception($"Could not parse {propertyNamingQuery} to {nameof(ResponsePropertyNamingPolicy)}");
        }

        var registeredSerializerOptions = context.HttpContext.RequestServices.GetService<IOptions<JsonSerializerOptions>>()?.Value;
#pragma warning disable CA1869 // Cache and reuse 'JsonSerializerOptions' instances
        var serializerOptions = new JsonSerializerOptions(registeredSerializerOptions
            ?? _options)
        {
            PropertyNamingPolicy = ResolveJsonNamingPolicy(namingPolicy)
        };
#pragma warning restore CA1869 // Cache and reuse 'JsonSerializerOptions' instances

        using var buffer = new MemoryStream();
        buffer.Seek(0, SeekOrigin.Begin);
        await JsonSerializer.SerializeAsync(buffer, _value, serializerOptions);

        var response = context.HttpContext.Response;
        response.StatusCode = (int)_statusCode;
        response.ContentType = "application/json";
        response.ContentLength = buffer.Length;

        await response.StartAsync();
        using var bodyWriterStream = response.BodyWriter.AsStream();
        buffer.WriteTo(bodyWriterStream);
        await response.BodyWriter.CompleteAsync();
        await response.CompleteAsync();
    }

    private static JsonNamingPolicy ResolveJsonNamingPolicy(ResponsePropertyNamingPolicy namingPolicy) =>
        namingPolicy switch
        {
            ResponsePropertyNamingPolicy.SnakeCaseUpper => JsonNamingPolicy.SnakeCaseUpper,
            ResponsePropertyNamingPolicy.SnakeCaseLower => JsonNamingPolicy.SnakeCaseLower,
            ResponsePropertyNamingPolicy.KebabCaseUpper => JsonNamingPolicy.KebabCaseUpper,
            ResponsePropertyNamingPolicy.KebabCaseLower => JsonNamingPolicy.KebabCaseLower,
            ResponsePropertyNamingPolicy.CapitalCase => new JsonCapitalCaseNamingPolicy(),
            _ => JsonNamingPolicy.CamelCase,
        };
}
