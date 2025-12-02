using Yarp.ReverseProxy.Configuration;
using Yarp.ReverseProxy.Forwarder;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Yarp.ReverseProxy.Transforms;

var builder = WebApplication.CreateBuilder(args);

var complaintsBase = builder.Configuration["COMPLAINTS_BASE_URL"] ?? "https://localhost:7031";
var tenantsBase = builder.Configuration["TENANTS_BASE_URL"] ?? "http://localhost:5006";
var usersBase = builder.Configuration["USERS_BASE_URL"] ?? "http://localhost:5007";
var authBase = builder.Configuration["AUTH_BASE_URL"] ?? "http://localhost:5008";

var authClientBuilder = builder.Services.AddHttpClient("auth", c => c.BaseAddress = new Uri(authBase));
if (builder.Environment.IsDevelopment())
{
 authClientBuilder.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
 {
 ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
 });
}

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy
            .SetIsOriginAllowed(origin =>
                origin.StartsWith("https://localhost:") || 
                origin.StartsWith("http://localhost:")
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services.AddReverseProxy()
 .AddTransforms(transformBuilderContext =>
 {
     transformBuilderContext.AddRequestTransform(async transformContext =>
     {
         var logger = transformContext.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
         
         // Forward tenant and user info from HttpContext.Items to headers
         if (transformContext.HttpContext.Items.TryGetValue("TenantId", out var tenantId) && tenantId != null)
         {
             logger.LogInformation("Adding X-Tenant-Id header: {TenantId}", tenantId);
             transformContext.ProxyRequest.Headers.TryAddWithoutValidation("X-Tenant-Id", tenantId.ToString()!);
         }
         else
         {
             logger.LogWarning("TenantId not found in HttpContext.Items");
         }
         
         if (transformContext.HttpContext.Items.TryGetValue("UserId", out var userId) && userId != null)
         {
             logger.LogInformation("Adding X-User-Id header: {UserId}", userId);
             transformContext.ProxyRequest.Headers.TryAddWithoutValidation("X-User-Id", userId.ToString()!);
         }
         await Task.CompletedTask;
     });
 })
 .LoadFromMemory(
 routes: new[]
 {
 new RouteConfig
 {
 RouteId = "complaints-route",
 ClusterId = "complaints",
 Match = new RouteMatch { Path = "/complaints/{**catch-all}" }
 },
 new RouteConfig
 {
 RouteId = "tenants-route",
 ClusterId = "tenants",
 Match = new RouteMatch { Path = "/tenants/{**catch-all}" }
 },
 new RouteConfig
 {
 RouteId = "users-route",
 ClusterId = "users",
 Match = new RouteMatch { Path = "/users/{**catch-all}" }
 },
 new RouteConfig
 {
 RouteId = "auth-route",
 ClusterId = "auth",
 Match = new RouteMatch { Path = "/auth/{**catch-all}" }
 }
 },
 clusters: new[]
 {
 new ClusterConfig
 {
 ClusterId = "complaints",
 Destinations = new Dictionary<string, DestinationConfig>
 {
 ["primary"] = new DestinationConfig { Address = complaintsBase.TrimEnd('/') + "/" }
 }
 },
 new ClusterConfig
 {
 ClusterId = "tenants",
 Destinations = new Dictionary<string, DestinationConfig>
 {
 ["primary"] = new DestinationConfig { Address = tenantsBase.TrimEnd('/') + "/" }
 }
 },
 new ClusterConfig
 {
 ClusterId = "users",
 Destinations = new Dictionary<string, DestinationConfig>
 {
 ["primary"] = new DestinationConfig { Address = usersBase.TrimEnd('/') + "/" }
 }
 },
 new ClusterConfig
 {
 ClusterId = "auth",
 Destinations = new Dictionary<string, DestinationConfig>
 {
 ["primary"] = new DestinationConfig { Address = authBase.TrimEnd('/') + "/" }
 }
 }
 }
 );

var app = builder.Build();

app.UseCors();

// Strip any spoofed internal trust header coming from outside.
app.Use(async (ctx, next) =>
{
 if (ctx.Request.Headers.ContainsKey("X-Internal-Trusted"))
 ctx.Request.Headers.Remove("X-Internal-Trusted");
 await next();
});

// Authentication middleware: protect all routes except login/register.
app.Use(async (ctx, next) =>
{
    // Ignore middleware for login/register/ping/root
    var path = ctx.Request.Path.Value?.ToLowerInvariant() ?? string.Empty;
    if (path.StartsWith("/auth/login") || path.StartsWith("/auth/ping") || path.StartsWith("/auth/register") || path == "/" || path.StartsWith("/tenants/list"))
    {
        await next();
        return;
    }
        
    var authHeader = ctx.Request.Headers["Authorization"].FirstOrDefault();
    if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer "))
    {
        ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
        await ctx.Response.WriteAsync("Missing bearer token");
        return;
    }
        
    var rawToken = authHeader.Substring("Bearer ".Length).Trim();
    JwtSecurityToken? jwtToken = null;
    
     try
     {
        var handler = new JwtSecurityTokenHandler();
        jwtToken = handler.ReadToken(rawToken) as JwtSecurityToken;
    }
    catch
    {
        ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
        await ctx.Response.WriteAsync("Invalid token format");
        return;
    }
    
    if (jwtToken == null)
    {
        ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
        await ctx.Response.WriteAsync("Invalid token");
        return;
    }

    var sid = jwtToken.Claims.FirstOrDefault(c => c.Type == "sid")?.Value;
    var tenantClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "tenant")?.Value;
    var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;

    if (string.IsNullOrWhiteSpace(sid) || string.IsNullOrWhiteSpace(tenantClaim) || string.IsNullOrWhiteSpace(userIdClaim))
    {
        ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
        await ctx.Response.WriteAsync("Missing claims");
        return;
    }
    
    var headerTenant = ctx.Request.Headers["x-tenant-id"].FirstOrDefault();
    if (!string.IsNullOrWhiteSpace(headerTenant) && !string.Equals(headerTenant, tenantClaim, StringComparison.OrdinalIgnoreCase))
    {
        ctx.Response.StatusCode = StatusCodes.Status403Forbidden;
        await ctx.Response.WriteAsync("Tenant mismatch");
        return;
    }

    var clientFactory = ctx.RequestServices.GetRequiredService<IHttpClientFactory>();
    var client = clientFactory.CreateClient("auth");
    var sessionValidate = await client.GetAsync($"/auth/sessions/{sid}?token={Uri.EscapeDataString(rawToken)}");
    if (!sessionValidate.IsSuccessStatusCode)
    {
        ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
        await ctx.Response.WriteAsync("Invalid session");
        return;
    }

 // Attach user context for downstream services if needed
 ctx.Items["UserId"] = userIdClaim;
 ctx.Items["TenantId"] = tenantClaim;

 await next();
});

app.MapGet("/", () => "CMS Gateway is running");

app.MapReverseProxy();

app.Run();
