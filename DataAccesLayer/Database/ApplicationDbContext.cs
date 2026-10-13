using Microsoft.EntityFrameworkCore;

namespace DataAccesLayer.Database;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<PlannerUser> Users => Set<PlannerUser>();
    public DbSet<PlannerRole> Roles => Set<PlannerRole>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PlannerUser>(entity =>
        {
            entity.ToTable("User");
            entity.HasKey(user => user.UserId);
            entity.Property(user => user.UserId).ValueGeneratedOnAdd();
            entity.Property(user => user.Name).HasMaxLength(100).IsRequired();
            entity.Property(user => user.Email).HasMaxLength(255).IsRequired();
            entity.Property(user => user.PasswordHash).HasMaxLength(255).IsRequired();
            entity.HasOne(user => user.Role)
                .WithMany(role => role.Users)
                .HasForeignKey(user => user.RoleId);
        });

        modelBuilder.Entity<PlannerRole>(entity =>
        {
            entity.ToTable("Role");
            entity.HasKey(role => role.RoleId);
            entity.Property(role => role.RoleId).ValueGeneratedOnAdd();
            entity.Property(role => role.Name).HasMaxLength(50).IsRequired();
            entity.Property(role => role.Description).HasMaxLength(255);
        });
    }
}