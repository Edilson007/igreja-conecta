using IgrejaConecta.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;

namespace IgrejaConecta.Api.Controllers;

[ApiController]
[Route("api/admin/parishes")]
public sealed class AdminParishesController(ChurchDbContext db, IConfiguration configuration) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult> Create([FromHeader(Name = "X-Admin-Key")] string? adminKey, [FromBody] CreateParishRequest request, CancellationToken ct)
    {
        var expectedKey = configuration["Admin:Key"];
        if (string.IsNullOrWhiteSpace(expectedKey) || !string.Equals(adminKey, expectedKey, StringComparison.Ordinal))
            return Unauthorized(new { message = "Chave administrativa inválida." });

        if (string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.City) || string.IsNullOrWhiteSpace(request.Sector))
            return BadRequest(new { message = "Nome, cidade e setor são obrigatórios." });

        var parish = new ParishRecord
        {
            Id = Guid.NewGuid(), Name = request.Name.Trim(), City = request.City.Trim(), State = string.IsNullOrWhiteSpace(request.State) ? "MG" : request.State.Trim().ToUpperInvariant(),
            Sector = request.Sector.Trim(), Address = request.Address?.Trim() ?? string.Empty, Phone = request.Phone?.Trim(), ImageUrl = request.ImageUrl?.Trim(),
            IsPremium = request.IsPremium, LastScheduleConfirmation = DateTimeOffset.UtcNow
        };
        parish.MassSchedules.AddRange(request.MassSchedules.Where(x => !string.IsNullOrWhiteSpace(x.Day) && !string.IsNullOrWhiteSpace(x.Time)).Select(x => new MassScheduleRecord { ParishId = parish.Id, Day = x.Day, Time = x.Time, Description = x.Description?.Trim() }));
        db.Parishes.Add(parish);
        await db.SaveChangesAsync(ct);
        return Created($"/api/parishes/{parish.Id}", new { parish.Id });
    }
}

public sealed record CreateParishRequest(string Name, string Sector, string City, string? State, string? Address, string? Phone, string? ImageUrl, bool IsPremium, IReadOnlyCollection<CreateMassScheduleRequest> MassSchedules);
public sealed record CreateMassScheduleRequest(string Day, string Time, string? Description);
