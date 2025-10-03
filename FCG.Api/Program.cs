using FCG.Infrastructure.Data;
using FCG.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

# region DB 
builder.AddDatabase();
#endregion

# region Identity
// 2️ Configuração do Identity
builder.AddIdentity(); // Identity + Roles (Admin, Player)
#endregion

//  Configuração de autenticação JWT
// builder.AddJwtAuthentication(); // JWT + Claims

# region Services
// Add service defaults & Aspire components.
builder.AddServiceDefaults();
# endregion

// Add services to the container.
builder.Services.AddProblemDetails();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

app.MapDefaultEndpoints();

// using (var scope = app.Services.CreateScope())
// {
//   var dbContext = scope.ServiceProvider.GetRequiredService<AppIdentityDbContext>();
//   await dbContext.Database.MigrateAsync();
// }

await app.RunAsync();