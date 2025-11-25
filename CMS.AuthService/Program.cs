using CMS.AuthService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddHttpClient();

var connectionString = builder.Configuration.GetConnectionString("authdb")
    ?? "Data Source=auth.db;Cache=Shared";

builder.Services.AddDbContext<AuthDbContext>(o => o.UseSqlite(connectionString));

var app = builder.Build();

if (app.Environment.IsDevelopment()) app.MapOpenApi();

app.MapDefaultEndpoints();
app.MapGet("/ping", () => Results.Ok("Running"));
app.MapControllers();
app.Run();
