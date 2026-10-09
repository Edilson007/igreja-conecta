using Microsoft.EntityFrameworkCore;

namespace IgrejaConecta.Infrastructure.Persistence;

public sealed class ChurchDbContext(DbContextOptions<ChurchDbContext> options) : DbContext(options)
{
    public DbSet<ParishRecord> Parishes => Set<ParishRecord>();
    public DbSet<MassScheduleRecord> MassSchedules => Set<MassScheduleRecord>();
    public DbSet<ActivityRecord> Activities => Set<ActivityRecord>();
    public DbSet<ChapelRecord> Chapels => Set<ChapelRecord>();
    public DbSet<CommunityRecord> Communities => Set<CommunityRecord>();
    public DbSet<ParishUserRecord> ParishUsers => Set<ParishUserRecord>();
    public DbSet<PlusContentRecord> PlusContents => Set<PlusContentRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ParishRecord>(entity =>
        {
            entity.ToTable("Parishes");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).HasMaxLength(200).IsRequired();
            entity.Property(x => x.Sector).HasMaxLength(100).IsRequired();
            entity.Property(x => x.ImageUrl).HasMaxLength(1000);
            entity.HasIndex(x => new { x.City, x.Sector });
            entity.HasMany(x => x.MassSchedules).WithOne().HasForeignKey(x => x.ParishId).OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(x => x.Activities).WithOne().HasForeignKey(x => x.ParishId).OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(x => x.Chapels).WithOne().HasForeignKey(x => x.ParishId).OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(x => x.Communities).WithOne().HasForeignKey(x => x.ParishId).OnDelete(DeleteBehavior.Cascade);
        });
        modelBuilder.Entity<MassScheduleRecord>().ToTable("MassSchedules");
        modelBuilder.Entity<CommunityRecord>(entity =>
        {
            entity.ToTable("Communities");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).HasMaxLength(200).IsRequired();
            entity.Property(x => x.ImageUrl).HasMaxLength(1000);
            entity.HasMany(x => x.MassSchedules).WithOne().HasForeignKey(x => x.CommunityId).OnDelete(DeleteBehavior.Cascade);
        });
        modelBuilder.Entity<ActivityRecord>().ToTable("Activities");
        modelBuilder.Entity<ChapelRecord>().ToTable("Chapels");
        modelBuilder.Entity<ParishUserRecord>(entity =>
        {
            entity.ToTable("ParishUsers");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).HasMaxLength(160).IsRequired();
            entity.Property(x => x.Email).HasMaxLength(320).IsRequired();
            entity.Property(x => x.PasswordHash).HasMaxLength(1000).IsRequired();
            entity.Property(x => x.Role).HasMaxLength(40).IsRequired();
            entity.HasIndex(x => new { x.ParishId, x.Email }).IsUnique();
        });
        modelBuilder.Entity<PlusContentRecord>(entity =>
        {
            entity.ToTable("PlusContents");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Category).HasMaxLength(40).IsRequired();
            entity.Property(x => x.Status).HasMaxLength(40).IsRequired();
            entity.Property(x => x.Title).HasMaxLength(200).IsRequired();
            entity.Property(x => x.ImageUrl).HasMaxLength(1000);
            entity.HasIndex(x => new { x.ParishId, x.Status, x.Category });
        });
    }
}

public sealed class ParishRecord
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Sector { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = "MG";
    public string Address { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsPremium { get; set; }
    public DateTimeOffset LastScheduleConfirmation { get; set; }
    public List<MassScheduleRecord> MassSchedules { get; set; } = [];
    public List<ActivityRecord> Activities { get; set; } = [];
    public List<ChapelRecord> Chapels { get; set; } = [];
    public List<CommunityRecord> Communities { get; set; } = [];
}
public sealed class MassScheduleRecord { public int Id { get; set; } public Guid ParishId { get; set; } public Guid? CommunityId { get; set; } public string Day { get; set; } = string.Empty; public string Time { get; set; } = string.Empty; public string? Description { get; set; } public string? Frequency { get; set; } }
public sealed class CommunityRecord { public Guid Id { get; set; } public Guid ParishId { get; set; } public string Name { get; set; } = string.Empty; public string Address { get; set; } = string.Empty; public string? Phone { get; set; } public string? ImageUrl { get; set; } public List<MassScheduleRecord> MassSchedules { get; set; } = []; }
public sealed class ActivityRecord { public int Id { get; set; } public Guid ParishId { get; set; } public string Title { get; set; } = string.Empty; public DateTimeOffset StartsAt { get; set; } public string? Description { get; set; } }
public sealed class ChapelRecord { public int Id { get; set; } public Guid ParishId { get; set; } public string Name { get; set; } = string.Empty; public string? Address { get; set; } }
public sealed class ParishUserRecord { public Guid Id { get; set; } public Guid ParishId { get; set; } public string Name { get; set; } = string.Empty; public string Email { get; set; } = string.Empty; public string PasswordHash { get; set; } = string.Empty; public string Role { get; set; } = ParishRole.Editor; public bool IsActive { get; set; } = true; public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow; }
public static class ParishRole { public const string Administrator = "Administrator"; public const string Editor = "Editor"; public const string Publisher = "Publisher"; }
public sealed class PlusContentRecord { public Guid Id { get; set; } public Guid ParishId { get; set; } public Guid CreatedByUserId { get; set; } public Guid? ReviewedByUserId { get; set; } public string Category { get; set; } = PlusCategory.Event; public string Status { get; set; } = PlusStatus.Draft; public string Title { get; set; } = string.Empty; public DateTimeOffset? EventStartsAt { get; set; } public string? DayOfWeek { get; set; } public string? Time { get; set; } public string? Location { get; set; } public string? ImageUrl { get; set; } public string? Description { get; set; } public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow; public DateTimeOffset? PublishedAt { get; set; } }
public static class PlusCategory { public const string Event = "Event"; public const string PrayerGroup = "PrayerGroup"; public const string YouthGroup = "YouthGroup"; public const string Notice = "Notice"; }
public static class PlusStatus { public const string Draft = "Draft"; public const string PendingReview = "PendingReview"; public const string Published = "Published"; public const string ChangesRequested = "ChangesRequested"; }
