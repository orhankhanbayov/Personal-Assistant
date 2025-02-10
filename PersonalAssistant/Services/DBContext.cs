using Microsoft.EntityFrameworkCore;
using PersonalAssistant.Models;

namespace PersonalAssistant.Services;

public class AppDbContext : DbContext
{
	public AppDbContext(DbContextOptions<AppDbContext> options)
		: base(options) { }

	public DbSet<User> Users { get; set; }
	public DbSet<EventDetails> Events { get; set; }
	public DbSet<TaskDetails> Tasks { get; set; }
	public DbSet<Notification> Notifications { get; set; }
	public DbSet<Category> Categories { get; set; }
	public DbSet<AuditLog> AuditLogs { get; set; }
	public DbSet<CallHistory> CallHistories { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		// Users
		modelBuilder.Entity<User>(entity =>
		{
			entity.HasKey(u => u.UserID);
			entity.Property(u => u.FirstName).IsRequired().HasMaxLength(50);
			entity.Property(u => u.LastName).IsRequired().HasMaxLength(50);
			entity.Property(u => u.Email).IsRequired().HasMaxLength(100);
			entity.Property(u => u.PasswordHash).IsRequired().HasMaxLength(255);
			entity.Property(u => u.PhoneNumber).IsRequired().HasMaxLength(15);
			entity
				.HasMany(u => u.CallHistories)
				.WithOne(c => c.User)
				.HasForeignKey(c => c.UserID)
				.OnDelete(DeleteBehavior.NoAction);
			entity.HasIndex(u => u.Email).IsUnique();
		});

		// Events
		modelBuilder.Entity<EventDetails>(entity =>
		{
			entity.HasKey(e => e.EventID);
			entity.Property(e => e.Title).IsRequired().HasMaxLength(100);
			entity.Property(e => e.Description).HasMaxLength(255);
			entity.Property(e => e.Location).HasMaxLength(150);

			entity
				.HasOne(e => e.User)
				.WithMany(u => u.Events)
				.HasForeignKey(e => e.UserID)
				.OnDelete(DeleteBehavior.Cascade);
		});

		// Tasks
		modelBuilder.Entity<TaskDetails>(entity =>
		{
			entity.HasKey(t => t.TaskID);
			entity.Property(t => t.Title).IsRequired().HasMaxLength(100);
			entity.Property(t => t.Description).HasMaxLength(255);
			entity.Property(t => t.Priority).HasMaxLength(50);

			entity
				.HasOne(t => t.User)
				.WithMany(u => u.Tasks)
				.HasForeignKey(t => t.UserID)
				.OnDelete(DeleteBehavior.Cascade);
		});

		// Notifications
		modelBuilder.Entity<Notification>(entity =>
		{
			entity.HasKey(n => n.NotificationID);
			entity.Property(n => n.Message).IsRequired().HasMaxLength(255);
			entity.Property(n => n.SentAt).IsRequired();

			entity
				.HasOne(n => n.User)
				.WithMany(u => u.Notifications)
				.HasForeignKey(c => c.UserID)
				.OnDelete(DeleteBehavior.NoAction);
			entity
				.HasOne(n => n.EventDetail)
				.WithMany(e => e.Notifications)
				.HasForeignKey(c => c.EventID)
				.OnDelete(DeleteBehavior.NoAction);

			entity
				.HasOne(n => n.TaskDetail)
				.WithMany(t => t.Notifications)
				.HasForeignKey(n => n.TaskID)
				.OnDelete(DeleteBehavior.NoAction);
		});

		// Categories
		modelBuilder.Entity<Category>(entity =>
		{
			entity.HasKey(c => c.CategoryID);
			entity.Property(c => c.Name).IsRequired().HasMaxLength(50);
			entity
				.HasOne(c => c.User)
				.WithMany(u => u.Categories)
				.HasForeignKey(c => c.UserID)
				.OnDelete(DeleteBehavior.Cascade);
		});

		// AuditLog
		modelBuilder.Entity<AuditLog>(entity =>
		{
			entity.HasKey(a => a.LogID);
			entity.Property(a => a.Action).IsRequired().HasMaxLength(100);
			entity.Property(a => a.TableName).IsRequired().HasMaxLength(50);
			entity.Property(a => a.Timestamp).IsRequired();
			entity.Property(a => a.Details).IsRequired();

			entity.HasOne(a => a.User).WithMany(u => u.AuditLogs).HasForeignKey(al => al.UserID);
		});

		// CallHistory

		modelBuilder.Entity<CallHistory>(entity =>
		{
			entity.HasKey(c => c.CallHistoryId);
			entity.Property(c => c.CallSid).IsRequired().HasMaxLength(50);
			entity.Property(c => c.From).IsRequired().HasMaxLength(15);
			entity.Property(c => c.To).IsRequired().HasMaxLength(15);
			entity.Property(c => c.StartTime).IsRequired();
			entity.Property(c => c.EndTime).IsRequired();
		});
	}
}
