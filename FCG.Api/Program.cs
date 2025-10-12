using FCG.Infrastructure.Extensions.App;
using FCG.Infrastructure.Extensions.Builder;

var builder = WebApplication.CreateBuilder(args);

builder.AddInfrastructure();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddProblemDetails();
builder.Services.AddSwaggerGen(); // Register Swagger services

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

await app.ConfigurePipeline();

app.MapDefaultEndpoints();

await app.RunAsync();