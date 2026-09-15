using Microsoft.AspNetCore.Mvc;

namespace IgrejaConecta.Api.Controllers;

[ApiController]
[Route("api/admin/images")]
public sealed class AdminImagesController(IConfiguration configuration, IWebHostEnvironment environment) : ControllerBase
{
    private const long MaxFileSize = 5 * 1024 * 1024;
    private static readonly Dictionary<string, string> AllowedTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        ["image/jpeg"] = ".jpg",
        ["image/png"] = ".png",
        ["image/webp"] = ".webp"
    };

    [HttpPost]
    [RequestSizeLimit(MaxFileSize)]
    public async Task<ActionResult<ImageUploadResponse>> Upload(
        [FromHeader(Name = "X-Admin-Key")] string? key,
        IFormFile? file,
        CancellationToken ct)
    {
        if (!Authorized(key)) return Unauthorized();
        if (file is null || file.Length == 0) return BadRequest(new { message = "Selecione uma imagem." });
        if (file.Length > MaxFileSize) return BadRequest(new { message = "A imagem deve ter no máximo 5 MB." });
        if (!AllowedTypes.TryGetValue(file.ContentType, out var extension))
            return BadRequest(new { message = "Envie uma imagem JPG, PNG ou WEBP." });

        var uploadsDirectory = Path.Combine(environment.WebRootPath, "uploads");
        Directory.CreateDirectory(uploadsDirectory);
        var fileName = $"{Guid.NewGuid():N}{extension}";
        await using var output = System.IO.File.Create(Path.Combine(uploadsDirectory, fileName));
        await file.CopyToAsync(output, ct);
        return Ok(new ImageUploadResponse($"/uploads/{fileName}"));
    }

    private bool Authorized(string? key) => !string.IsNullOrWhiteSpace(configuration["Admin:Key"]) && string.Equals(key, configuration["Admin:Key"], StringComparison.Ordinal);
}

public sealed record ImageUploadResponse(string Url);
