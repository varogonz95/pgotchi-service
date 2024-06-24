using FluentValidation;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Pgotchi.Shared;
using Pgotchi.WebApi;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

// Add services to the container.
services
    .AddControllers()
    .AddNewtonsoftJson(setup =>
    {
        var namingStrategy = new CamelCaseNamingStrategy();
        setup.SerializerSettings.Converters.Add(new StringEnumConverter());
    });


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();
services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly(), ServiceLifetime.Transient);
services.AddAutoMapper(config =>
{
    config.AddProfile<DataProfile>();
});

AppServiceConfiguration.RegisterServices(services);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors(cors =>
    {
        cors.AllowAnyOrigin();
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseRouting();

app.MapControllers();

await app.RunAsync();
