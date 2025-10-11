using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace FCG.Infrastructure.Extensions.Builder;

public static class AuthorizationExtensions
{
    public static IHostApplicationBuilder AddAuthorizationJWT(this IHostApplicationBuilder builder)
    {
        var jwtSettings = builder.Configuration.GetSection("Jwt");

        if (string.IsNullOrEmpty(jwtSettings["SecretKey"]))
            throw new InvalidOperationException("JWT SecretKey não configurado.");

        var secretKey = Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!);

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false; // true em produção
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings["Issuer"],
                ValidAudience = jwtSettings["Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(secretKey),
                ClockSkew = TimeSpan.Zero // Remove tolerância de tempo
            };

            // Para Swagger/API testing
            options.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                    {
                        context.Response?.Headers?.Add("Token-Expired", "true");
                    }
                    return Task.CompletedTask;
                }
            };
        });

        return builder;
    }
}
