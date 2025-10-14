using FCG.Infrastructure.Extensions.App;
using FCG.Infrastructure.Extensions.Builder;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.AddInfrastructure();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddProblemDetails();
builder.Services.AddSwaggerGen(); 

builder.Services.ConfigureHttpClientDefaults(static http =>
{
    // Turn on service discovery by default
    http.AddServiceDiscovery();
});

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();
app.UseExceptionHandler();

await app.ConfigurePipeline();
app.MapDefaultEndpoints();

await app.RunAsync();
