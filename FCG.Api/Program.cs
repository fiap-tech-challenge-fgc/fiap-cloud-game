using FCG.Infrastructure.Extensions.App;
using FCG.Infrastructure.Extensions.Builder;

var builder = WebApplication.CreateBuilder(args);

builder.AddInfrastructure();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddProblemDetails();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.MapDefaultEndpoints();

await app.RunAsync();