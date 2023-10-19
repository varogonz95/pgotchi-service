using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Pgotchi.Apis.Converters;

namespace Pgotchi.Apis;

public static class ServiceDependencies
{
    public static WebApplicationBuilder ConfigureServices(this WebApplicationBuilder builder)
    {
        // Add services to the container.
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddControllers()
            .AddNewtonsoftJson(options =>
            {
                var converterIndex = options.SerializerSettings.Converters.Where(x => x.GetType().IsAssignableTo(typeof(KeyValuePair<object, object>))).Select((_, i) => i).FirstOrDefault(-1);
                
                if (converterIndex >= 0) 
                    options.SerializerSettings.Converters.RemoveAt(converterIndex);

                var namingStrategy = new CamelCaseNamingStrategy(processDictionaryKeys: true, overrideSpecifiedNames: true);
                options.SerializerSettings.Converters.Add(new StringEnumConverter(namingStrategy, allowIntegerValues: false));
                options.SerializerSettings.Converters.Add(new CustomKeyValuePairConverter());
                options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
            });

        return builder;
    }
}
