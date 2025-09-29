var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.fiap_cloud_game_api_ApiService>("apiservice");

builder.AddProject<Projects.fiap_cloud_game_api_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService);

builder.Build().Run();
