using Pgotchi.Shared.Options;
using Pgotchi.Shared.Services;

namespace Pgotchi.WebApi;

public static class AppServiceConfiguration
{
    public static void RegisterServices(IServiceCollection services)
    {
        services.AddOptionsWithValidateOnStart<AzureIotHubOptions>()
            .BindConfiguration(AzureIotHubOptions.SectionName);

        services.AddTransient<IDeviceService, DeviceService>();

        //services.AddScoped(provider =>
        //{
        //    var iotHubOptions = provider.GetRequiredService<IOptions<AzureIotHubOptions>>();
        //    string connectionString = iotHubOptions.Value?.ConnectionString ?? throw new Exception($"{nameof(AzureIotHubOptions)} could not be located");
        //    return RegistryManager.CreateFromConnectionString(connectionString);
        //});
    }
}
