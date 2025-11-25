using CMS.TenantService.Infrastructure.Data;
using CMS.TenantService.Controllers;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var connectionString = builder.Configuration.GetConnectionString("tenantsdb")
    ?? "Data Source=tenants.db";

builder.Services.AddDbContext<TenantDbContext>(o => o.UseSqlite(connectionString));

var app = builder.Build();
if (app.Environment.IsDevelopment()) app.MapOpenApi();

// Internal verification middleware
app.Use(async (ctx, next) =>
{
    var requireInternal = ctx.GetEndpoint()?.Metadata.GetMetadata<RequireInternalAttribute>() != null;
    var internalTrusted = ctx.Request.Headers.TryGetValue("X-Internal-Trusted", out var v) && v == "true";
    if (requireInternal && !internalTrusted)
    {
        ctx.Response.StatusCode = 403;
        await ctx.Response.WriteAsync("Internal trust required");
        return;
    }
    await next();
});

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TenantDbContext>();
    
    if (!db.Tenants.Any())
    {
        var natwestId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        var hsbcId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
        db.Tenants.AddRange(
            new CMS.TenantService.Domain.Tenant { TenantId = natwestId, Code = "NATWEST", Name = "NatWest Bank" },
            new CMS.TenantService.Domain.Tenant { TenantId = hsbcId, Code = "HSBC", Name = "HSBC" }
        );
        db.SaveChanges();
    }
}

app.MapControllers();
app.Run();