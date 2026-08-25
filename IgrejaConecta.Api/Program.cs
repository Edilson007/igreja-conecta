using IgrejaConecta.Application.Parishes;
using IgrejaConecta.Domain.Repositories;
using IgrejaConecta.Infrastructure.Repositories;
using IgrejaConecta.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddCors(options => options.AddDefaultPolicy(policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=igreja-conecta.db";
builder.Services.AddDbContext<ChurchDbContext>(options =>
{
    if (connectionString.StartsWith("postgres", StringComparison.OrdinalIgnoreCase)) options.UseNpgsql(ToNpgsqlConnectionString(connectionString));
    else options.UseSqlite(connectionString);
});
builder.Services.AddScoped<IParishRepository, EfParishRepository>();
builder.Services.AddScoped<ParishService>();

var app = builder.Build();
using (var scope = app.Services.CreateScope())
    await DatabaseInitializer.InitializeAsync(scope.ServiceProvider.GetRequiredService<ChurchDbContext>());
app.UseCors();
app.UseDefaultFiles();
app.UseStaticFiles();
app.MapGet("/health", () => Results.Ok(new { status = "ok" }));
app.MapControllers();
app.MapFallbackToFile("index.html");
app.Run();

static string ToNpgsqlConnectionString(string databaseUrl)
{
    if (!Uri.TryCreate(databaseUrl, UriKind.Absolute, out var uri)) return databaseUrl;

    var credentials = uri.UserInfo.Split(':', 2);
    return new Npgsql.NpgsqlConnectionStringBuilder
    {
        Host = uri.Host,
        Port = uri.IsDefaultPort ? 5432 : uri.Port,
        Database = uri.AbsolutePath.Trim('/'),
        Username = Uri.UnescapeDataString(credentials[0]),
        Password = credentials.Length > 1 ? Uri.UnescapeDataString(credentials[1]) : string.Empty
    }.ConnectionString;
}
