var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("mainpostgres")
    .WithEnvironment("POSTGRES_DB", "maindb")
    .WithPgAdmin();

var postgresDb = postgres.AddDatabase("DbFcg");


builder.AddProject<Projects.FCG_Api>("Api")
    .WithReference(postgres)
    .WaitFor(postgres);

builder.AddProject<Projects.FCG_Blazor>("Blazor");

// builder.AddProject<Projects.fiap_cloud_game_api_Web>("Application")
//     .WithExternalHttpEndpoints()
//     .WithReference(apiService);

await builder.Build().RunAsync();
