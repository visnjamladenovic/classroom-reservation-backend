using ClassroomReservationBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace ClassroomReservationBackend.Data;
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Classroom> Classrooms => Set<Classroom>();
    public DbSet<Reservation> Reservations => Set<Reservation>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // === USER ===
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(u => u.Email).IsUnique();
        });

        // === CLASSROOM ===
        modelBuilder.Entity<Classroom>(entity =>
        {
            entity.HasIndex(c => c.ClassroomType);
        });

        // === RESERVATION ===
        modelBuilder.Entity<Reservation>(entity =>
        {
            entity.HasIndex(r => r.Status);
            entity.HasIndex(r => new { r.StartTime, r.EndTime });

            // User who created the reservation
            entity.HasOne(r => r.User)
                .WithMany(u => u.Reservations)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // User who approved the reservation
            entity.HasOne(r => r.ApprovedByUser)
                .WithMany(u => u.ApprovedReservations)
                .HasForeignKey(r => r.ApprovedBy)
                .OnDelete(DeleteBehavior.SetNull);

            // Classroom
            entity.HasOne(r => r.Classroom)
                .WithMany(c => c.Reservations)
                .HasForeignKey(r => r.ClassroomId)
                .OnDelete(DeleteBehavior.Cascade);

            // Check constraint: end_time > start_time
            entity.ToTable(t => t.HasCheckConstraint("chk_reservation_time", "end_time > start_time"));
        });

        // === REFRESH TOKEN ===
        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasIndex(rt => rt.Token).IsUnique();

            entity.HasOne(rt => rt.User)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    // Auto-update UpdatedAt on save
    public override int SaveChanges()
    {
        UpdateTimestamps();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateTimestamps()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            var updatedAtProp = entry.Properties.FirstOrDefault(p => p.Metadata.Name == "UpdatedAt");
            if (updatedAtProp != null)
            {
                updatedAtProp.CurrentValue = DateTime.UtcNow;
            }
        }
    }
}
