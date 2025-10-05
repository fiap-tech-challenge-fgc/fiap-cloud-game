var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithDataVolume("FCG-Data");

var db = postgres.AddDatabase("DbFcg");

postgres.WithPgAdmin(pgAdmin =>
{
    pgAdmin.WaitFor(postgres); 
    pgAdmin.WaitFor(db);       
});

builder.AddProject<Projects.FCG_Api>("Api")
    .WithReference(db)
    .WaitFor(db);

builder.AddProject<Projects.FCG_Blazor>("Blazor");


// builder.AddProject<Projects.fiap_cloud_game_api_Web>("Application")
//     .WithExternalHttpEndpoints()
//     .WithReference(apiService);

await builder.Build().RunAsync();
