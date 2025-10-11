using FCG.Infrastructure.Extensions.App;
using FCG.Infrastructure.Extensions.Builder;

var builder = WebApplication.CreateBuilder(args);

builder.AddInfrastructure();

// Add services to the container.
builder.Services.AddProblemDetails();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

await app.ApplyMigrationsAsync();

app.MapDefaultEndpoints();

await app.RunAsync();