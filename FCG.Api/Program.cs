using FCG.Domain.Enums;
using FCG.Infrastructure.Extensions.App;
using FCG.Infrastructure.Extensions.Builder;

var builder = WebApplication.CreateBuilder(args);

builder.AddInfrastructure(ProjectType.Api);

var app = builder.Build();

await app.ConfigurePipeline(ProjectType.Api);

await app.RunAsync();

// ✅ Adicione esta linha para permitir testes de integração
public partial class Program { }

