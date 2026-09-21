using IgrejaConecta.Application.Parishes;
using Microsoft.AspNetCore.Mvc;

namespace IgrejaConecta.Api.Controllers;

[ApiController]
[Route("api/parishes")]
public sealed class ParishesController(ParishService service) : ControllerBase
{
    [HttpGet]
    public Task<IReadOnlyCollection<ParishDto>> Search([FromQuery] string? city, [FromQuery] string? sector, [FromQuery] string? query, CancellationToken ct) => service.SearchAsync(city, sector, query, ct);

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ParishDto>> GetById(Guid id, CancellationToken ct)
    {
        var parish = await service.GetByIdAsync(id, ct);
        return parish is { IsPremium: true } ? Ok(parish) : NotFound();
    }
}
