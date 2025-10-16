using FCG.Blazor;
using FCG.Blazor.Components;
using FCG.Infrastructure.Enums;
using FCG.Infrastructure.Extensions.App;
using FCG.Infrastructure.Extensions.Builder;

var builder = WebApplication.CreateBuilder(args);

builder.AddInfrastructure(ProjectType.Blazor);

builder.Services.AddOutputCache();

builder.Services.AddHttpClient<WeatherApiClient>(client =>
{
    // This URL uses "https+http://" to indicate HTTPS is preferred over HTTP.
    // Learn more about service discovery scheme resolution at https://aka.ms/dotnet/sdschemes.
    client.BaseAddress = new("https+http://fcg-api");
});

var app = builder.Build();

await app.ConfigurePipeline(ProjectType.Blazor);

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapDefaultEndpoints();

app.Run();
