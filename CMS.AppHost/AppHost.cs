using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

// Removed Postgres resources; each service now uses local SQLite file DB.
var complaintService = builder.AddProject<Projects.CMS_ComplaintService>("complaint-service"); // internal only

var tenantService = builder.AddProject<Projects.CMS_TenantService>("tenant-service"); // internal only

var userService = builder.AddProject<Projects.CMS_UserService>("user-service"); // internal only

var authService = builder.AddProject<Projects.CMS_AuthService>("auth-service")
    .WithExternalHttpEndpoints()
    .WithEnvironment("TENANTS_BASE_URL", tenantService.GetEndpoint("https"))
    .WithEnvironment("USERS_BASE_URL", userService.GetEndpoint("https"))
    .WithEnvironment("JWT_SIGNING_KEY", "dev-super-secret-key-at-least-32-chars!")
    .WithEnvironment("Auth:Jwt:Issuer", "cms.auth")
    .WithEnvironment("Auth:Jwt:Audience", "cms.services");

var gateway = builder.AddProject<Projects.CMS_Web>("gateway")
    .WithReference(complaintService)
    .WithReference(tenantService)
    .WithReference(userService)
    .WithReference(authService)
    .WithExternalHttpEndpoints()
    .WithEnvironment("COMPLAINTS_BASE_URL", complaintService.GetEndpoint("https"))
    .WithEnvironment("AUTH_BASE_URL", authService.GetEndpoint("https"))
    .WithEnvironment("TENANTS_BASE_URL", tenantService.GetEndpoint("https"))
    .WithEnvironment("USERS_BASE_URL", userService.GetEndpoint("https"))
    .WithEnvironment("JWT_PUBLIC_ISSUER", "cms.auth")
    .WithEnvironment("JWT_EXPECTED_AUDIENCE", "cms.services");


builder.AddNpmApp("helpdesk", "../CMS.HelpDesk.Frontend", "dev")
    .WithNpmPackageInstallation()
    .WithReference(gateway)
    .WithHttpEndpoint(env: "PORT")
    .WithExternalHttpEndpoints()
    .WithEnvironment("NUXT_PUBLIC_API_BASE", gateway.GetEndpoint("https"));

builder.Build().Run();
