using System.Collections.Concurrent;
using System.Security.Cryptography;
using IgrejaConecta.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IgrejaConecta.Api.Controllers;

[ApiController]
[Route("api/portal")]
public sealed class ParishPortalController(ChurchDbContext db) : ControllerBase
{
    private static readonly ConcurrentDictionary<string, PortalSession> Sessions = new();

    [HttpPost("login")]
    public async Task<ActionResult<PortalLoginDto>> Login(PortalLoginRequest request, CancellationToken ct)
    {
        var user = await db.ParishUsers.SingleOrDefaultAsync(x => x.ParishId == request.ParishId && x.Email == request.Email.Trim().ToLowerInvariant() && x.IsActive, ct);
        if (user is null || !Passwords.Verify(request.Password, user.PasswordHash)) return Unauthorized();
        var token = Convert.ToHexString(RandomNumberGenerator.GetBytes(32));
        Sessions[token] = new PortalSession(user.Id, user.ParishId, user.Role, DateTimeOffset.UtcNow.AddHours(12));
        return Ok(new PortalLoginDto(token, user.Name, user.Role, user.ParishId));
    }

    [HttpGet("contents")]
    public async Task<ActionResult<IReadOnlyCollection<PortalContentDto>>> List([FromHeader(Name = "X-Portal-Token")] string? token, CancellationToken ct)
    {
        var session = Require(token); if (session is null) return Unauthorized();
        return Ok(await db.PlusContents.Where(x => x.ParishId == session.ParishId).OrderByDescending(x => x.CreatedAt).Select(x => ToDto(x)).ToArrayAsync(ct));
    }

    [HttpPost("contents")]
    public async Task<ActionResult> Create([FromHeader(Name = "X-Portal-Token")] string? token, PortalContentRequest request, CancellationToken ct)
    {
        var session = Require(token); if (session is null) return Unauthorized();
        if (!Valid(request)) return BadRequest("Título e categoria são obrigatórios.");
        db.PlusContents.Add(new PlusContentRecord { Id = Guid.NewGuid(), ParishId = session.ParishId, CreatedByUserId = session.UserId, Category = request.Category, Title = request.Title.Trim(), EventStartsAt = request.EventStartsAt, DayOfWeek = request.DayOfWeek?.Trim(), Time = request.Time?.Trim(), Location = request.Location?.Trim(), ImageUrl = request.ImageUrl?.Trim(), Description = request.Description?.Trim(), Status = PlusStatus.Draft });
        await db.SaveChangesAsync(ct); return NoContent();
    }

    [HttpPost("contents/{id:guid}/submit")]
    public async Task<ActionResult> Submit(Guid id, [FromHeader(Name = "X-Portal-Token")] string? token, CancellationToken ct)
    {
        var session = Require(token); if (session is null) return Unauthorized(); var item = await Find(id, session.ParishId, ct); if (item is null) return NotFound(); item.Status = PlusStatus.PendingReview; await db.SaveChangesAsync(ct); return NoContent();
    }

    [HttpPost("contents/{id:guid}/publish")]
    public async Task<ActionResult> Publish(Guid id, [FromHeader(Name = "X-Portal-Token")] string? token, CancellationToken ct)
    {
        var session = Require(token); if (session is null) return Unauthorized(); if (session.Role is not (ParishRole.Administrator or ParishRole.Publisher)) return Forbid(); var item = await Find(id, session.ParishId, ct); if (item is null) return NotFound(); item.Status = PlusStatus.Published; item.ReviewedByUserId = session.UserId; item.PublishedAt = DateTimeOffset.UtcNow; await db.SaveChangesAsync(ct); return NoContent();
    }

    private PortalSession? Require(string? token) => token is not null && Sessions.TryGetValue(token, out var session) && session.ExpiresAt > DateTimeOffset.UtcNow ? session : null;
    private Task<PlusContentRecord?> Find(Guid id, Guid parishId, CancellationToken ct) => db.PlusContents.SingleOrDefaultAsync(x => x.Id == id && x.ParishId == parishId, ct);
    private static bool Valid(PortalContentRequest r) => !string.IsNullOrWhiteSpace(r.Title) && new[] { PlusCategory.Event, PlusCategory.PrayerGroup, PlusCategory.YouthGroup, PlusCategory.Notice }.Contains(r.Category);
    private static PortalContentDto ToDto(PlusContentRecord x) => new(x.Id, x.Category, x.Status, x.Title, x.EventStartsAt, x.DayOfWeek, x.Time, x.Location, x.ImageUrl, x.Description, x.CreatedAt);
}

public sealed record PortalLoginRequest(Guid ParishId, string Email, string Password);
public sealed record PortalLoginDto(string Token, string Name, string Role, Guid ParishId);
public sealed record PortalContentRequest(string Category, string Title, DateTimeOffset? EventStartsAt, string? DayOfWeek, string? Time, string? Location, string? ImageUrl, string? Description);
public sealed record PortalContentDto(Guid Id, string Category, string Status, string Title, DateTimeOffset? EventStartsAt, string? DayOfWeek, string? Time, string? Location, string? ImageUrl, string? Description, DateTimeOffset CreatedAt);
sealed record PortalSession(Guid UserId, Guid ParishId, string Role, DateTimeOffset ExpiresAt);
static class Passwords { public static bool Verify(string password, string hash) { var p=hash.Split(':'); if(p.Length!=2)return false; var salt=Convert.FromBase64String(p[0]); var actual=Rfc2898DeriveBytes.Pbkdf2(password,salt,120000,HashAlgorithmName.SHA256,32); return CryptographicOperations.FixedTimeEquals(actual,Convert.FromBase64String(p[1])); } }
