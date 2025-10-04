using FCG.Infrastructure.Extensions;
using FCG.Infrastructure.Seeds;

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

#region Migrations
// using (var scope = app.Services.CreateScope())
// {
//   var dbContext = scope.ServiceProvider.GetRequiredService<AppIdentityDbContext>();
//   await dbContext.Database.MigrateAsync();
// }

//using (var scope = app.Services.CreateScope())
//{
//    var services = scope.ServiceProvider;
//    await services.SeedAppIdentityAsync();
//}
#endregion



await app.RunAsync();