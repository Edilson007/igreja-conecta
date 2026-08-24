namespace IgrejaConecta.Domain.Entities;
public sealed record MassSchedule(DayOfWeek Day, TimeOnly Time, string? Description = null);
