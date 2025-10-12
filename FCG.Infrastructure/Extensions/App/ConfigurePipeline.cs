using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

namespace FCG.Infrastructure.Extensions.App;

public static class ConfigurePipelineExtensions
{
    public static async Task<WebApplication> ConfigurePipeline(this WebApplication app)
    {
        app.UseExceptionHandler();
        await app.ApplyMigrationsAsync();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        return app;
    }
}
