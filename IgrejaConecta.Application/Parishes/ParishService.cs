using IgrejaConecta.Domain.Entities;
using IgrejaConecta.Domain.Repositories;
namespace IgrejaConecta.Application.Parishes;
public sealed class ParishService(IParishRepository repository)
{
    public async Task<IReadOnlyCollection<ParishDto>> SearchAsync(string? city, string? sector, string? query, CancellationToken ct) => (await repository.SearchAsync(city, sector, query, ct)).Select(Map).ToArray();
    public async Task<ParishDto?> GetByIdAsync(Guid id, CancellationToken ct) { var parish = await repository.GetByIdAsync(id, ct); return parish is null ? null : Map(parish); }
    private static ParishDto Map(Parish p) => new(p.Id, p.Name, p.Sector, p.City, p.State, p.Address, p.Phone, p.IsPremium, p.LastScheduleConfirmation, p.MassSchedules.Select(x => new MassScheduleDto(x.Day.ToString(), x.Time.ToString("HH:mm"), x.Description)).ToArray(), p.Activities.Select(x => new ActivityDto(x.Title, x.StartsAt, x.Description)).ToArray(), p.Chapels.Select(x => new ChapelDto(x.Name, x.Address)).ToArray());
}
