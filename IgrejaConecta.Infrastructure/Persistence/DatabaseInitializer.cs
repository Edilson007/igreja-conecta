using Microsoft.EntityFrameworkCore;

namespace IgrejaConecta.Infrastructure.Persistence;

public static class DatabaseInitializer
{
    public static async Task InitializeAsync(ChurchDbContext db, CancellationToken ct = default)
    {
        await db.Database.EnsureCreatedAsync(ct);
        if (await db.Parishes.AnyAsync(ct)) return;

        var confirmedAt = new DateTimeOffset(2026, 8, 22, 9, 0, 0, TimeSpan.FromHours(-3));
        db.Parishes.AddRange(
            Create("11111111-1111-1111-1111-111111111111", "Catedral Diocesana Nossa Senhora das Dores", "Setor Guaxupé", "Guaxupé", confirmedAt, [("Sunday", "08:00"), ("Sunday", "19:00")], "Comunidade de teste — São José"),
            Create("22222222-2222-2222-2222-222222222222", "Basílica Nossa Senhora da Saúde", "Setor Poços de Caldas", "Poços de Caldas", confirmedAt, [("Saturday", "18:00"), ("Sunday", "10:00")], "Comunidade de teste — Nossa Senhora Aparecida"),
            Create("33333333-3333-3333-3333-333333333333", "Paróquia Nossa Senhora Aparecida", "Setor Passos", "Passos", confirmedAt, [("Sunday", "09:00")], null),
            Create("44444444-4444-4444-4444-444444444444", "Paróquia Divino Espírito Santo", "Setor Cássia", "Delfinópolis", confirmedAt, [("Sunday", "08:30")], null));
        await db.SaveChangesAsync(ct);
    }

    private static ParishRecord Create(string id, string name, string sector, string city, DateTimeOffset confirmedAt, (string Day, string Time)[] schedules, string? chapel) => new()
    {
        Id = Guid.Parse(id), Name = name, Sector = sector, City = city, Address = "Centro — endereço de teste", IsPremium = true, LastScheduleConfirmation = confirmedAt,
        MassSchedules = schedules.Select(x => new MassScheduleRecord { Day = x.Day, Time = x.Time, Description = "Horário de teste — confirmar com a paróquia" }).ToList(),
        Chapels = chapel is null ? [] : [new ChapelRecord { Name = chapel }]
    };
}
