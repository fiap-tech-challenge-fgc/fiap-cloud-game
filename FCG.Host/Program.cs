var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("Postgres")
    .WithDataVolume("pgData");

var apiService = builder.AddProject<Projects.FCG_Api>("Api")
    .WithReference(postgres);

// builder.AddProject<Projects.fiap_cloud_game_api_Web>("Application")
//     .WithExternalHttpEndpoints()
//     .WithReference(apiService);

await builder.Build().RunAsync();
