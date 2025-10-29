using FCG.Application.Interfaces;
using FCG.Application.Interfaces.Repository;
using FCG.Application.Interfaces.Service;
using FCG.Application.Repositories;
using FCG.Application.Services;
using FCG.Application.Services.Auth;
using FCG.Domain.Data;
using FCG.Domain.Enums;
using FCG.Domain.Service;
using FCG.Infrastructure.Initializer;
using FCG.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace FCG.Infrastructure.Extensions.Builder;

public static class ServicesExtensions
{
    public static IHostApplicationBuilder AddInfrastructure(this IHostApplicationBuilder builder, ProjectType projectType)
    {
        switch (projectType)
        {
            case ProjectType.Api:
                builder.ConfigureApiServices();
                break;

            case ProjectType.Blazor:
                builder.ConfigureBlazorServices();
                break;

            case ProjectType.Host:
                builder.ConfigureHostServices();
                break;

            case ProjectType.Application:
                builder.ConfigureApplicationServices();
                break;

            default:
                break;
        }

        builder.AddServiceDefaults(projectType);

        return builder;
    }

    private static IHostApplicationBuilder ConfigureApiServices(this IHostApplicationBuilder builder)
    {
        builder.AddDatabase();
        builder.AddIdentity();
        builder.AddAuthorizationJWT();
        builder.AddAuthorizationPolicies();

        builder.Services.AddScoped(typeof(IdentityDAL<>)); // para identidade
        builder.Services.AddScoped(typeof(IDAL<>), typeof(DomainDAL<>)); // padrão para domínio
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<IPlayerRepository, PlayerRepository>();
        builder.Services.AddScoped<IGameRepository, GameRepository>();
        builder.Services.AddScoped<IGalleryRepository, GalleryRepository>();
        builder.Services.AddScoped<ILibraryRepository, LibraryRepository>();
        builder.Services.AddScoped<ICartRepository, CartRepository>();
        builder.Services.AddScoped<IPurchaseRepository, PurchaseRepository>();

        builder.Services.AddScoped<ICartDomainService, CartDomainService>();
        builder.Services.AddScoped<IPurchaseDomainService, PurchaseDomainService>();

        builder.Services.AddScoped<ISeedService, SeedService>();
        builder.Services.AddScoped<IInfrastructureInitializer, InfrastructureInitializer>();
        builder.Services.AddScoped<IJwtService, JwtService>();
        builder.Services.AddScoped<IAuthService, AuthService>();
        builder.Services.AddScoped<IPlayerService, PlayerService>();
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<IGameService, GameService>();
        builder.Services.AddScoped<IGalleryService, GalleryService>();
        builder.Services.AddScoped<ICartService, CartService>();
        builder.Services.AddScoped<ILibraryService, LibraryService>();
        builder.Services.AddScoped<IPurchaseService, PurchaseService>();


        builder.Services.AddControllers();
        builder.Services.AddExceptionHandler();
        builder.Services.AddSwaggerDocumentation();

        builder.Services.ConfigureHttpClientDefaults(static http =>
        {
            // Turn on service discovery by default
            http.AddServiceDiscovery();
        });

        return builder;
    }

    private static IHostApplicationBuilder ConfigureBlazorServices(this IHostApplicationBuilder builder)
    {
        // Add services to the container.
        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();

        builder.Services.AddOutputCache();

        return builder;
    }

    private static IHostApplicationBuilder ConfigureHostServices(this IHostApplicationBuilder builder)
    {
        // Configurações específicas do Host, se houver
        return builder;
    }

    private static IHostApplicationBuilder ConfigureApplicationServices(this IHostApplicationBuilder builder)
    {
        // Configurações específicas do Application, se houver
        return builder;
    }

    private static IHostApplicationBuilder AddServiceDefaults(this IHostApplicationBuilder builder, ProjectType projectType)
    {
        builder.ConfigureOpenTelemetry();

        builder.AddDefaultHealthChecks();

        builder.Services.AddServiceDiscovery();

        builder.Services.ConfigureHttpClientDefaults(http =>
        {
            http.AddStandardResilienceHandler();
            http.AddServiceDiscovery();
        });

        return builder;
    }

    public static IHostApplicationBuilder ConfigureOpenTelemetry(this IHostApplicationBuilder builder)
    {
        builder.Logging.AddOpenTelemetry(logging =>
        {
            logging.IncludeFormattedMessage = true;
            logging.IncludeScopes = true;
        });

        builder.Services.AddOpenTelemetry()
            .WithMetrics(metrics =>
            {
                metrics.AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation();
            })
            .WithTracing(tracing =>
            {
                tracing.AddAspNetCoreInstrumentation()
                    //.AddGrpcClientInstrumentation() // se usar gRPC
                    .AddHttpClientInstrumentation();
            });

        builder.AddOpenTelemetryExporters();

        return builder;
    }

    private static IHostApplicationBuilder AddOpenTelemetryExporters(this IHostApplicationBuilder builder)
    {
        var useOtlpExporter = !string.IsNullOrWhiteSpace(builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]);

        if (useOtlpExporter)
        {
            builder.Services.AddOpenTelemetry().UseOtlpExporter();
        }

        // Exemplo para Azure Monitor (descomente se usar)
        //if (!string.IsNullOrEmpty(builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"]))
        //{
        //    builder.Services.AddOpenTelemetry()
        //       .UseAzureMonitor();
        //}

        return builder;
    }

    public static IHostApplicationBuilder AddDefaultHealthChecks(this IHostApplicationBuilder builder)
    {
        builder.Services.AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy(), new[] { "live" });

        return builder;
    }

    public static WebApplication MapDefaultEndpoints(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.MapHealthChecks("/health");

            app.MapHealthChecks("/alive", new HealthCheckOptions
            {
                Predicate = r => r.Tags.Contains("live")
            });
        }

        return app;
    }
}
