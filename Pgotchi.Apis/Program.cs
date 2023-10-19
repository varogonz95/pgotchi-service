using Pgotchi.Apis;

var builder = WebApplication.CreateBuilder(args);

var app = builder
    .ConfigureServices()
    .Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    //app.UseSwaggerUI(options =>
    //{
    //    options.SwaggerEndpoint("/swagger/v1/swagger.json", $"{Assembly.GetExecutingAssembly().GetName().Name}");
    //    options.DisplayRequestDuration();

    //    var option = new RewriteOptions();
    //    option.AddRedirect("^$", "swagger/index.html");
    //    app.UseRewriter(option);
    //});
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();