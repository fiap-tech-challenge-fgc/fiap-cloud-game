var builder = DistributedApplication.CreateBuilder(args);

#if DEBUG
    var postgres = builder.AddPostgres("Postgres")
        .WithDataVolume("pgData")
        .WithEnvironment("POSTGRES_HOST_AUTH_METHOD", "trust") 
        .WithEnvironment("POSTGRES_PASSWORD", "postgres")
        .WithEnvironment("POSTGRES_USER", "postgres")
        .WithEnvironment("POSTGRES_DB", "MeuBanco");

builder.AddProject<Projects.FCG_Api>("Api")
    .WithReference(postgres);
#else
    var postgres = builder.AddPostgres("Postgres")
        .WithDataVolume("pgData");
#endif


builder.AddProject<Projects.FCG_Blazor>("Blazor");

// builder.AddProject<Projects.fiap_cloud_game_api_Web>("Application")
//     .WithExternalHttpEndpoints()
//     .WithReference(apiService);

await builder.Build().RunAsync();
