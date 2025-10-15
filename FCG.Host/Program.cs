var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder
    .AddPostgres("postgres")
    .WithDataVolume("FCG-Data");

var db = postgres
    .AddDatabase("DbFcg");

postgres.WithPgAdmin(pgAdmin =>
{
    pgAdmin.WaitFor(postgres);
    pgAdmin.WaitFor(db);
});

var api = builder.AddProject<Projects.FCG_Api>("fcg-api")
    .WithReference(db)
    .WaitFor(db)
    .WithHttpHealthCheck("/health");

builder.AddProject<Projects.FCG_Blazor>("fcg-blazor")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(api)
    .WaitFor(api);

// builder.AddProject<Projects.fiap_cloud_game_api_Web>("Application")
//     .WithExternalHttpEndpoints()
//     .WithReference(apiService);

await builder.Build().RunAsync();
