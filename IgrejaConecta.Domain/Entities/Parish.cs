namespace IgrejaConecta.Domain.Entities;

public sealed class Parish
{
    public Parish(Guid id, string name, string sector, string city, string state, string address, string? phone, string? imageUrl, bool isPremium, DateTimeOffset lastScheduleConfirmation)
    { Id = id; Name = name; Sector = sector; City = city; State = state; Address = address; Phone = phone; ImageUrl = imageUrl; IsPremium = isPremium; LastScheduleConfirmation = lastScheduleConfirmation; }
    public Guid Id { get; }
    public string Name { get; }
    public string Sector { get; }
    public string City { get; }
    public string State { get; }
    public string Address { get; }
    public string? Phone { get; }
    public string? ImageUrl { get; }
    public bool IsPremium { get; }
    public DateTimeOffset LastScheduleConfirmation { get; }
    public IReadOnlyCollection<MassSchedule> MassSchedules => _massSchedules;
    public IReadOnlyCollection<Activity> Activities => _activities;
    public IReadOnlyCollection<Chapel> Chapels => _chapels;
    public IReadOnlyCollection<Community> Communities => _communities;
    private readonly List<MassSchedule> _massSchedules = [];
    private readonly List<Activity> _activities = [];
    private readonly List<Chapel> _chapels = [];
    private readonly List<Community> _communities = [];
    public void AddMassSchedule(MassSchedule schedule) => _massSchedules.Add(schedule);
    public void AddActivity(Activity activity) => _activities.Add(activity);
    public void AddChapel(Chapel chapel) => _chapels.Add(chapel);
    public void AddCommunity(Community community) => _communities.Add(community);
}
