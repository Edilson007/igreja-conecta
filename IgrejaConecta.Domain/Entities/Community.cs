namespace IgrejaConecta.Domain.Entities;

public sealed class Community
{
    public Community(Guid id, string name, string address, string? phone, string? imageUrl)
    { Id = id; Name = name; Address = address; Phone = phone; ImageUrl = imageUrl; }
    public Guid Id { get; }
    public string Name { get; }
    public string Address { get; }
    public string? Phone { get; }
    public string? ImageUrl { get; }
    public IReadOnlyCollection<MassSchedule> MassSchedules => _massSchedules;
    private readonly List<MassSchedule> _massSchedules = [];
    public void AddMassSchedule(MassSchedule schedule) => _massSchedules.Add(schedule);
}
