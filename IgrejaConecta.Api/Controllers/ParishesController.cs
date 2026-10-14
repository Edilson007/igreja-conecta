using IgrejaConecta.Application.Parishes;
using Microsoft.AspNetCore.Mvc;
using IgrejaConecta.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace IgrejaConecta.Api.Controllers;

[ApiController]
[Route("api/parishes")]
public sealed class ParishesController(ParishService service, ChurchDbContext db) : ControllerBase
{
    [HttpGet("{id:guid}/contents")]
    public async Task<ActionResult<IReadOnlyCollection<PortalContentDto>>> Contents(Guid id, CancellationToken ct)
    {
        if (!await db.Parishes.AnyAsync(x => x.Id == id && x.IsPremium, ct)) return NotFound();
        var contents = await db.PlusContents.AsNoTracking()
            .Where(x => x.ParishId == id && x.Status == PlusStatus.Published).ToArrayAsync(ct);
        return Ok(contents.OrderByDescending(x => x.PublishedAt ?? x.CreatedAt)
            .Select(x => new PortalContentDto(x.Id, x.Category, x.Status, x.Title, x.EventStartsAt,
                x.DayOfWeek, x.Time, x.Location, x.ImageUrl, x.Description, x.CreatedAt)).ToArray());
    }

    [HttpGet]
    public Task<IReadOnlyCollection<ParishDto>> Search([FromQuery] string? city, [FromQuery] string? sector, [FromQuery] string? query, CancellationToken ct) => service.SearchAsync(city, sector, query, ct);

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ParishDto>> GetById(Guid id, CancellationToken ct)
    {
        var parish = await service.GetByIdAsync(id, ct);
        return parish is { IsPremium: true } ? Ok(parish) : NotFound();
    }

    [HttpGet("slug/{slug}")]
    public async Task<ActionResult<ParishDto>> GetBySlug(string slug, CancellationToken ct)
    {
        var parish = await service.GetBySlugAsync(slug, ct);
        return parish is null ? NotFound() : Ok(parish);
    }
}
