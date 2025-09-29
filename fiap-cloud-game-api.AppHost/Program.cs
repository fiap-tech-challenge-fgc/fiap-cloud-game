var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("DefaultPostgres")
    .WithDataVolume("pgData");

var apiService = builder.AddProject<Projects.fiap_cloud_game_api_ApiService>("apiservice")
    .WithReference(postgres);

// builder.AddProject<Projects.fiap_cloud_game_api_Web>("webfrontend")
//     .WithExternalHttpEndpoints()
//     .WithReference(apiService);

await builder.Build().RunAsync();
