using BusinessLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataAccesLayer.Database;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<SportEvent> SportEvents => Set<SportEvent>();
    public DbSet<Activity> Activities => Set<Activity>();
    public DbSet<Location> Locations => Set<Location>();
    public DbSet<TimeSlot> TimeSlots => Set<TimeSlot>();
    public DbSet<Planning> Plannings => Set<Planning>();
    public DbSet<ActivitySchedule> ActivitySchedules => Set<ActivitySchedule>();
    public DbSet<Availability> Availabilities => Set<Availability>();
    public DbSet<Registration> Registrations => Set<Registration>();
    public DbSet<VolunteerAssignment> VolunteerAssignments => Set<VolunteerAssignment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SportEvent>(entity =>
        {
            entity.ToTable("SportEvent");
            entity.HasKey(eventEntity => eventEntity.EventId);
            entity.Property(eventEntity => eventEntity.EventName).HasMaxLength(200).IsRequired();
            entity.Property(eventEntity => eventEntity.Description).HasMaxLength(1000);
        });

        modelBuilder.Entity<Activity>(entity =>
        {
            entity.ToTable("Activity");
            entity.HasKey(activity => activity.ActivityId);
            entity.Property(activity => activity.ActivityName).HasMaxLength(200).IsRequired();
            entity.Property(activity => activity.ActivityType).HasMaxLength(100).IsRequired();
            entity.Property(activity => activity.Description).HasMaxLength(1000).IsRequired();
            entity.HasOne(activity => activity.Event)
                .WithMany(eventEntity => eventEntity.Activities)
                .HasForeignKey(activity => activity.EventId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Location>(entity =>
        {
            entity.ToTable("Location");
            entity.HasKey(location => location.LocationId);
            entity.Property(location => location.Name)
                .HasColumnName("LocationName")
                .HasMaxLength(200)
                .IsRequired();
            entity.Property(location => location.LocationType).HasMaxLength(100).IsRequired();
            entity.Property(location => location.Description).HasMaxLength(1000).IsRequired();
        });

        modelBuilder.Entity<TimeSlot>(entity =>
        {
            entity.ToTable("TimeSlot");
            entity.HasKey(timeSlot => timeSlot.TimeSlotId);
            entity.HasOne(timeSlot => timeSlot.Event)
                .WithMany(eventEntity => eventEntity.TimeSlots)
                .HasForeignKey(timeSlot => timeSlot.EventId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Planning>(entity =>
        {
            entity.ToTable("Planning");
            entity.HasKey(planning => planning.PlanningId);
            entity.Property(planning => planning.Name).HasMaxLength(200).IsRequired();
            entity.Property(planning => planning.Status).HasMaxLength(50).IsRequired();
            entity.HasOne(planning => planning.Event)
                .WithMany(eventEntity => eventEntity.Plannings)
                .HasForeignKey(planning => planning.EventId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(planning => planning.CreatedByUser)
                .WithMany(user => user.CreatedPlannings)
                .HasForeignKey(planning => planning.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ActivitySchedule>(entity =>
        {
            entity.ToTable("ActivitySchedule");
            entity.HasKey(schedule => schedule.ActivityScheduleId);
            entity.HasOne(schedule => schedule.Planning)
                .WithMany(planning => planning.ActivitySchedules)
                .HasForeignKey(schedule => schedule.PlanningId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(schedule => schedule.Activity)
                .WithMany(activity => activity.ActivitySchedules)
                .HasForeignKey(schedule => schedule.ActivityId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(schedule => schedule.Location)
                .WithMany(location => location.ActivitySchedules)
                .HasForeignKey(schedule => schedule.LocationId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(schedule => schedule.TimeSlot)
                .WithMany(timeSlot => timeSlot.ActivitySchedules)
                .HasForeignKey(schedule => schedule.TimeSlotId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Availability>(entity =>
        {
            entity.ToTable("Availability");
            entity.HasKey(availability => availability.AvailabilityId);
            entity.HasOne(availability => availability.User)
                .WithMany(user => user.Availabilities)
                .HasForeignKey(availability => availability.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Registration>(entity =>
        {
            entity.ToTable("Registration");
            entity.HasKey(registration => registration.RegistrationId);
            entity.Property(registration => registration.Status).HasMaxLength(50).IsRequired();
            entity.HasOne(registration => registration.User)
                .WithMany(user => user.Registrations)
                .HasForeignKey(registration => registration.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(registration => registration.Activity)
                .WithMany(activity => activity.Registrations)
                .HasForeignKey(registration => registration.ActivityId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<VolunteerAssignment>(entity =>
        {
            entity.ToTable("VolunteerAssignment");
            entity.HasKey(assignment => assignment.AssignmentId);
            entity.HasOne(assignment => assignment.User)
                .WithMany(user => user.VolunteerAssignments)
                .HasForeignKey(assignment => assignment.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(assignment => assignment.Activity)
                .WithMany(activity => activity.VolunteerAssignments)
                .HasForeignKey(assignment => assignment.ActivityId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<User>(entity =>
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

        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("Role");
            entity.HasKey(role => role.RoleId);
            entity.Property(role => role.RoleId).ValueGeneratedOnAdd();
            entity.Property(role => role.Name).HasMaxLength(50).IsRequired();
            entity.Property(role => role.Description).HasMaxLength(255);
        });
    }
}