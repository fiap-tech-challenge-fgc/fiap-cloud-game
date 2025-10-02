using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace FCG.Infrastructure.Extensions;

public static class ConfigureSwaggerExtensions
{
    public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "FCG API",
                Version = "v1",
                Description = "Documentação da API do Fiap Cloud Game"
            });

            // Se quiser incluir comentários XML, descomente e configure o caminho:
            // var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            // var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            // c.IncludeXmlComments(xmlPath);
        });

        return services;
    }
}
