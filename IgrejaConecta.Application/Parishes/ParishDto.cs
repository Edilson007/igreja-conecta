namespace IgrejaConecta.Application.Parishes;
public sealed record ParishDto(Guid Id, string Name, string Sector, string City, string State, string Address, string? Phone, string? ImageUrl, bool IsPremium, DateTimeOffset LastScheduleConfirmation, IReadOnlyCollection<MassScheduleDto> MassSchedules, IReadOnlyCollection<ActivityDto> Activities, IReadOnlyCollection<ChapelDto> Chapels, IReadOnlyCollection<CommunityDto> Communities);
public sealed record MassScheduleDto(string Day, string Time, string? Description);
public sealed record CommunityDto(Guid Id, string Name, string Address, string? Phone, string? ImageUrl, IReadOnlyCollection<MassScheduleDto> MassSchedules);
public sealed record ActivityDto(string Title, DateTimeOffset StartsAt, string? Description);
public sealed record ChapelDto(string Name, string? Address);
