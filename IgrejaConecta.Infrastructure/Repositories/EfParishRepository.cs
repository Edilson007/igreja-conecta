using IgrejaConecta.Domain.Entities;
using IgrejaConecta.Domain.Repositories;
using IgrejaConecta.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace IgrejaConecta.Infrastructure.Repositories;

public sealed class EfParishRepository(ChurchDbContext db) : IParishRepository
{
    public async Task<Parish?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => (await IncludeChildren().SingleOrDefaultAsync(x => x.Id == id, ct)) is { } record ? Map(record) : null;

    public async Task<IReadOnlyCollection<Parish>> SearchAsync(string? city, string? sector, string? query, CancellationToken ct = default)
    {
        var source = IncludeChildren();
        if (!string.IsNullOrWhiteSpace(city)) source = source.Where(x => x.City.ToLower().Contains(city.ToLower()));
        if (!string.IsNullOrWhiteSpace(sector)) source = source.Where(x => x.Sector == sector);
        if (!string.IsNullOrWhiteSpace(query)) source = source.Where(x => x.Name.ToLower().Contains(query.ToLower()) || x.Address.ToLower().Contains(query.ToLower()));
        return (await source.OrderBy(x => x.Name).ToListAsync(ct)).Select(Map).ToArray();
    }

    private IQueryable<ParishRecord> IncludeChildren() => db.Parishes.AsNoTracking().Include(x => x.MassSchedules).Include(x => x.Activities).Include(x => x.Chapels).Include(x => x.Communities).ThenInclude(x => x.MassSchedules);
    private static Parish Map(ParishRecord source)
    {
        var parish = new Parish(source.Id, source.Name, source.Sector, source.City, source.State, source.Address, source.Phone, source.ImageUrl, source.IsPremium, source.LastScheduleConfirmation);
        foreach (var item in source.MassSchedules) parish.AddMassSchedule(new(Enum.Parse<DayOfWeek>(item.Day), TimeOnly.Parse(item.Time), item.Description));
        foreach (var item in source.Activities) parish.AddActivity(new(item.Title, item.StartsAt, item.Description));
        foreach (var item in source.Chapels) parish.AddChapel(new(item.Name, item.Address));
        foreach (var item in source.Communities)
        {
            var community = new Community(item.Id, item.Name, item.Address, item.Phone, item.ImageUrl);
            foreach (var schedule in item.MassSchedules) community.AddMassSchedule(new(Enum.Parse<DayOfWeek>(schedule.Day), TimeOnly.Parse(schedule.Time), schedule.Description));
            parish.AddCommunity(community);
        }
        return parish;
    }
}
