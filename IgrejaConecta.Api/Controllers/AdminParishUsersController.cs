using IgrejaConecta.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IgrejaConecta.Api.Controllers;

[ApiController]
[Route("api/admin/parishes/{parishId:guid}/users")]
public sealed class AdminParishUsersController(ChurchDbContext db, IConfiguration configuration) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult> List(Guid parishId, [FromHeader(Name = "X-Admin-Key")] string? key, CancellationToken ct)
    { if (!Authorized(key)) return Unauthorized(); return Ok(await db.ParishUsers.Where(x => x.ParishId == parishId).OrderBy(x => x.Name).Select(x => new { x.Id, x.Name, x.Email, x.Role, x.IsActive }).ToArrayAsync(ct)); }

    [HttpPost]
    public async Task<ActionResult> Create(Guid parishId, [FromHeader(Name = "X-Admin-Key")] string? key, ParishUserRequest request, CancellationToken ct)
    { if (!Authorized(key)) return Unauthorized(); if (string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.Email) || request.Password.Length < 10 || !Roles.Contains(request.Role)) return BadRequest("Informe nome, e-mail, perfil e senha com ao menos 10 caracteres."); if (!await db.Parishes.AnyAsync(x => x.Id == parishId, ct)) return NotFound(); db.ParishUsers.Add(new ParishUserRecord { Id = Guid.NewGuid(), ParishId = parishId, Name = request.Name.Trim(), Email = request.Email.Trim().ToLowerInvariant(), PasswordHash = Passwords.Hash(request.Password), Role = request.Role }); try { await db.SaveChangesAsync(ct); } catch (DbUpdateException) { return Conflict("Já existe um usuário com este e-mail nesta paróquia."); } return NoContent(); }
    private bool Authorized(string? key) => !string.IsNullOrWhiteSpace(configuration["Admin:Key"]) && string.Equals(key, configuration["Admin:Key"], StringComparison.Ordinal);
    private static readonly string[] Roles = [ParishRole.Administrator, ParishRole.Editor, ParishRole.Publisher];
}
public sealed record ParishUserRequest(string Name, string Email, string Password, string Role);
