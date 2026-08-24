namespace IgrejaConecta.Domain.Entities;
public sealed record Activity(string Title, DateTimeOffset StartsAt, string? Description = null);
