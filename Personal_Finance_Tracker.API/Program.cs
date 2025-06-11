using Personal_Finance_Tracker.API.Helper;
using Services.caching;
using Services.Service;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Add services to the container.
builder.Configuration.SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();

builder.Services.addDbServices(builder.Configuration)
                 .addAuth(builder.Configuration)
                 .addCustomServices();



var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapHub<NotificationHub>("Noti").RequireAuthorization();

var lifeTime = app.Lifetime;
var cacheService = app.Services.GetRequiredService<IcacheServices>();
lifeTime.ApplicationStopping.Register(() =>
{
    cacheService.removeAllKeys();
    app.Logger.LogInformation("Application is stopping. All cache keys have been removed.");
});


app.Run();
