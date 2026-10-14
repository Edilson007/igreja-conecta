using IgrejaConecta.Domain.Entities;
using IgrejaConecta.Domain.Repositories;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
namespace IgrejaConecta.Application.Parishes;
public sealed class ParishService(IParishRepository repository)
{
    public async Task<IReadOnlyCollection<ParishDto>> SearchAsync(string? city, string? sector, string? query, CancellationToken ct) => (await repository.SearchAsync(city, sector, query, ct)).Select(Map).ToArray();
    public async Task<ParishDto?> GetByIdAsync(Guid id, CancellationToken ct) { var parish = await repository.GetByIdAsync(id, ct); return parish is null ? null : Map(parish); }
    public async Task<ParishDto?> GetBySlugAsync(string slug, CancellationToken ct)
    {
        var matches = (await repository.SearchAsync(null, null, null, ct))
            .Where(x => x.IsPremium && Slugify($"{x.Name}-{x.City}").Equals(slug, StringComparison.OrdinalIgnoreCase))
            .Take(2).ToArray();
        return matches.Length == 1 ? Map(matches[0]) : null;
    }
    private static string Slugify(string value)
    {
        var decomposed = value.Normalize(NormalizationForm.FormD);
        var withoutDiacritics = new string(decomposed.Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark).ToArray());
        return Regex.Replace(withoutDiacritics.ToLowerInvariant(), "[^a-z0-9]+", "-").Trim('-');
    }
    private static ParishDto Map(Parish p) => new(p.Id, p.Name, p.Sector, p.City, p.State, p.Address, p.Phone, p.ImageUrl, p.IsPremium, p.LastScheduleConfirmation, p.MassSchedules.Select(x => new MassScheduleDto(x.Day.ToString(), x.Time.ToString("HH:mm"), x.Description, x.Frequency)).ToArray(), p.Activities.Select(x => new ActivityDto(x.Title, x.StartsAt, x.Description)).ToArray(), p.Chapels.Select(x => new ChapelDto(x.Name, x.Address)).ToArray(), p.Communities.Select(x => new CommunityDto(x.Id, x.Name, x.Address, x.Phone, x.ImageUrl, x.MassSchedules.Select(s => new MassScheduleDto(s.Day.ToString(), s.Time.ToString("HH:mm"), s.Description, s.Frequency)).ToArray())).ToArray());
}
