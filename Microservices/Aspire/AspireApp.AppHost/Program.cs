var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.App_Gateway_Api>("gatewayapi");

builder.AddProject<Projects.App_UserManagement_Api>("usermanagementapi")
    .WithExternalHttpEndpoints()
    .WithReference(apiService)
    .WaitFor(apiService);

builder.AddProject<Projects.App_SuperAdmin_Api>("superadminapi")
    .WithExternalHttpEndpoints()
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();
