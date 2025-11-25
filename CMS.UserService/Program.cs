using CMS.UserService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using CMS.UserService.Domain;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var connectionString = builder.Configuration.GetConnectionString("usersdb")
    ?? "Data Source=users.db";

builder.Services.AddDbContext<UserDbContext>(o => o.UseSqlite(connectionString));

var app = builder.Build();
if (app.Environment.IsDevelopment()) app.MapOpenApi();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<UserDbContext>();
    var force = app.Environment.IsDevelopment() && Environment.GetEnvironmentVariable("DEV_FORCE_MIGRATE") == "1";

    if (!db.Users.Any())
    {
        var natwest = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        var hsbc = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
        var agent1Id = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var support1Id = Guid.Parse("22222222-2222-2222-2222-222222222222");
        var agent2Id = Guid.Parse("33333333-3333-3333-3333-333333333333");
        db.Users.AddRange(
            new User { UserId = agent1Id, TenantId = natwest, Username = "agent1", Role = UserRole.Agent, PasswordHash = HashPassword("Passw0rd!") },
            new User { UserId = support1Id, TenantId = natwest, Username = "support1", Role = UserRole.Support, PasswordHash = HashPassword("Passw0rd!") },
            new User { UserId = agent2Id, TenantId = hsbc, Username = "agent2", Role = UserRole.Agent, PasswordHash = HashPassword("Passw0rd!") }
        );
        db.SaveChanges();
    }
}

app.MapControllers();
app.Run();

static string HashPassword(string password)
{
    var salt = System.Security.Cryptography.RandomNumberGenerator.GetBytes(16);
    using var derive = new System.Security.Cryptography.Rfc2898DeriveBytes(password, salt, 15_000, System.Security.Cryptography.HashAlgorithmName.SHA256);
    var hash = derive.GetBytes(32);
    return Convert.ToBase64String(salt) + ":" + Convert.ToBase64String(hash);
}