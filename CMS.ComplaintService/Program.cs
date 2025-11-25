using CMS.ComplaintService.Application.Handlers;
using CMS.ComplaintService.Application.Queries;
using CMS.ComplaintService.Domain.Entities;
using CMS.ComplaintService.Domain.Enums;
using CMS.ComplaintService.Infrastructure.Data;
using CMS.ComplaintService.Infrastructure.Messaging;
using CMS.ComplaintService.Infrastructure.Repositories;
using CMS.ComplaintService.Controllers;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers();

var connectionString = builder.Configuration.GetConnectionString("cmsdb")
    ?? builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Data Source=complaints.db";

builder.Services.AddDbContext<ComplaintDbContext>(options =>
{
    options.UseSqlite(connectionString);
});

builder.Services.AddScoped<IComplaintRepository, ComplaintRepository>();
builder.Services.AddScoped<LogComplaintHandler>();
builder.Services.AddScoped<IComplaintQueryService, ComplaintQueryService>();
builder.Services.AddSingleton<IEventBus, InMemoryEventBus>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

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

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
