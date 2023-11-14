using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureDefaults(args)
    .ConfigureFunctionsWorkerDefaults()
    .Build();

await host.RunAsync();
