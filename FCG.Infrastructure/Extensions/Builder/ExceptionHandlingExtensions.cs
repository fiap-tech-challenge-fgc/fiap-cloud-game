using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using FCG.Infrastructure.Middleware;

namespace FCG.Infrastructure.Extensions.Builder;

public static class ExceptionHandlingExtensions
{
    public static IServiceCollection AddExceptionHandler(this IServiceCollection services)
    {
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();
        
        return services;
    }

    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
    {
        app.UseExceptionHandler();
        app.UseStatusCodePages();
        
        return app;
    }
}