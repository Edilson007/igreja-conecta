using IgrejaConecta.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IgrejaConecta.Api.Controllers;

[ApiController]
[Route("api/admin/parishes")]
public sealed class AdminParishesController(ChurchDbContext db, IConfiguration configuration) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<AdminParishDto>>> List([FromHeader(Name = "X-Admin-Key")] string? key, CancellationToken ct)
    {
        if (!Authorized(key)) return Unauthorized();
        var parishes = await db.Parishes.AsNoTracking()
            .Include(x => x.MassSchedules)
            .OrderBy(x => x.Name)
            .ToListAsync(ct);

        return Ok(parishes.Select(x => new AdminParishDto(
            x.Id, x.Name, x.Sector, x.City, x.State, x.Address, x.Phone, x.ImageUrl, x.IsPremium,
            x.MassSchedules.OrderBy(s => s.Day).ThenBy(s => s.Time)
                .Select(s => new AdminMassScheduleDto(s.Day, s.Time, s.Description)).ToArray())).ToArray());
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromHeader(Name = "X-Admin-Key")] string? key, [FromBody] UpsertParishRequest request, CancellationToken ct)
    {
        if (!Authorized(key)) return Unauthorized();
        if (!Valid(request, out var error)) return BadRequest(new { message = error });
        var entity = new ParishRecord { Id = Guid.NewGuid() };
        Apply(entity, request); db.Parishes.Add(entity); await db.SaveChangesAsync(ct);
        return Created($"/api/parishes/{entity.Id}", new { entity.Id });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromHeader(Name = "X-Admin-Key")] string? key, [FromBody] UpsertParishRequest request, CancellationToken ct)
    {
        if (!Authorized(key)) return Unauthorized();
        if (!Valid(request, out var error)) return BadRequest(new { message = error });
        var entity = await db.Parishes.Include(x => x.MassSchedules).SingleOrDefaultAsync(x => x.Id == id, ct);
        if (entity is null) return NotFound();
        Apply(entity, request); await db.SaveChangesAsync(ct); return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, [FromHeader(Name = "X-Admin-Key")] string? key, CancellationToken ct)
    {
        if (!Authorized(key)) return Unauthorized();
        var entity = await db.Parishes.FindAsync([id], ct);
        if (entity is null) return NotFound();
        db.Parishes.Remove(entity); await db.SaveChangesAsync(ct); return NoContent();
    }

    private bool Authorized(string? key) => !string.IsNullOrWhiteSpace(configuration["Admin:Key"]) && string.Equals(key, configuration["Admin:Key"], StringComparison.Ordinal);
    private static bool Valid(UpsertParishRequest request, out string message) { message = "Nome, cidade e setor são obrigatórios."; return !string.IsNullOrWhiteSpace(request.Name) && !string.IsNullOrWhiteSpace(request.City) && !string.IsNullOrWhiteSpace(request.Sector); }
    private static void Apply(ParishRecord entity, UpsertParishRequest request)
    {
        entity.Name = request.Name.Trim(); entity.City = request.City.Trim(); entity.State = string.IsNullOrWhiteSpace(request.State) ? "MG" : request.State.Trim().ToUpperInvariant(); entity.Sector = request.Sector.Trim(); entity.Address = request.Address?.Trim() ?? string.Empty; entity.Phone = request.Phone?.Trim(); entity.ImageUrl = request.ImageUrl?.Trim(); entity.IsPremium = request.IsPremium; entity.LastScheduleConfirmation = DateTimeOffset.UtcNow;
        entity.MassSchedules.Clear(); foreach (var item in request.MassSchedules.Where(x => !string.IsNullOrWhiteSpace(x.Day) && !string.IsNullOrWhiteSpace(x.Time))) entity.MassSchedules.Add(new MassScheduleRecord { ParishId = entity.Id, Day = item.Day, Time = item.Time, Description = item.Description?.Trim() });
    }
}

public sealed record UpsertParishRequest(string Name, string Sector, string City, string? State, string? Address, string? Phone, string? ImageUrl, bool IsPremium, IReadOnlyCollection<UpsertMassScheduleRequest> MassSchedules);
public sealed record UpsertMassScheduleRequest(string Day, string Time, string? Description);
public sealed record AdminParishDto(Guid Id, string Name, string Sector, string City, string State, string Address, string? Phone, string? ImageUrl, bool IsPremium, IReadOnlyCollection<AdminMassScheduleDto> MassSchedules);
public sealed record AdminMassScheduleDto(string Day, string Time, string? Description);
