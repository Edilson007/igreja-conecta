using Microsoft.EntityFrameworkCore;

namespace IgrejaConecta.Infrastructure.Persistence;

public sealed class ChurchDbContext(DbContextOptions<ChurchDbContext> options) : DbContext(options)
{
    public DbSet<ParishRecord> Parishes => Set<ParishRecord>();
    public DbSet<MassScheduleRecord> MassSchedules => Set<MassScheduleRecord>();
    public DbSet<ActivityRecord> Activities => Set<ActivityRecord>();
    public DbSet<ChapelRecord> Chapels => Set<ChapelRecord>();
    public DbSet<CommunityRecord> Communities => Set<CommunityRecord>();

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
