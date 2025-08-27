using Microsoft.EntityFrameworkCore;
using Feedbacktool.Models;
using System.Collections.Generic;

namespace Feedbacktool;

public class ToolContext : DbContext
{
    public DbSet<Exercise> Exercises { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Subject> Subjects { get; set; } = null!;
    public DbSet<ClassGroup> ClassGroups { get; set; } = null!;
    public DbSet<ScoreGroup> ScoreGroups { get; set; } = null!;

    public ToolContext(DbContextOptions<ToolContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ---- Table-per-Type (TPT) mapping (you can keep this even if no inheritance now) ----
        modelBuilder.Entity<ClassGroup>().ToTable("ClassGroups");
        modelBuilder.Entity<ScoreGroup>().ToTable("ScoreGroups");

        // ---------- User <-> Subject (many-to-many) ----------
        modelBuilder.Entity<User>()
            .HasMany(u => u.Subjects)
            .WithMany(s => s.Users)
            .UsingEntity<Dictionary<string, object>>(
                "UserSubject",
                j => j.HasOne<Subject>().WithMany().HasForeignKey("SubjectId").OnDelete(DeleteBehavior.Cascade),
                j => j.HasOne<User>().WithMany().HasForeignKey("UserId").OnDelete(DeleteBehavior.Cascade),
                j =>
                {
                    j.HasKey("UserId", "SubjectId");
                    j.ToTable("UserSubjects");
                });

        // ---------- User -> ClassGroup (many-to-one, required) ----------
        modelBuilder.Entity<User>()
            .HasOne(u => u.ClassGroup)
            .WithMany(cg => cg.Users)
            .HasForeignKey(u => u.ClassGroupId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict); // don't cascade-delete users if a class group is removed

        // ---------- User <-> ScoreGroup (many-to-many) ----------
        // This ensures a composite PK (UserId, ScoreGroupId) -> one assignment max per pair.
        modelBuilder.Entity<User>()
            .HasMany(u => u.ScoreGroups)
            .WithMany(sg => sg.Users)
            .UsingEntity<Dictionary<string, object>>(
                "UserScoreGroup",
                j => j.HasOne<ScoreGroup>().WithMany().HasForeignKey("ScoreGroupId").OnDelete(DeleteBehavior.Cascade),
                j => j.HasOne<User>().WithMany().HasForeignKey("UserId").OnDelete(DeleteBehavior.Cascade),
                j =>
                {
                    j.HasKey("UserId", "ScoreGroupId");
                    j.ToTable("UserScoreGroups");
                });

        // ---------- ScoreGroup -> Subject (many-to-one) ----------
        modelBuilder.Entity<ScoreGroup>()
            .HasOne(sg => sg.Subject)
            .WithMany(s => s.ScoreGroups)   // ensure Subject has ICollection<ScoreGroup> ScoreGroups { get; set; }
            .HasForeignKey(sg => sg.SubjectId)
            .OnDelete(DeleteBehavior.Cascade);

        // ---------- Indexes / constraints ----------
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique(); // recommended: normalize/lowercase emails at write-time

        modelBuilder.Entity<User>().Property(u => u.Role).HasConversion<string>();

        // Optional but recommended: guard rails on strings
        modelBuilder.Entity<User>(e =>
        {
            e.Property(x => x.Name).IsRequired().HasMaxLength(200);
            e.Property(x => x.Email).IsRequired().HasMaxLength(320);
            e.Property(x => x.Password).IsRequired();
        });

        modelBuilder.Entity<ClassGroup>(e =>
        {
            e.Property(x => x.Name).IsRequired().HasMaxLength(200);
            // Optional: unique class group names
            // e.HasIndex(x => x.Name).IsUnique();
        });

        modelBuilder.Entity<ScoreGroup>(e =>
        {
            e.Property(x => x.Name).IsRequired().HasMaxLength(200);
            // Optional: enforce unique name per subject
            e.HasIndex(x => new { x.SubjectId, x.Name }).IsUnique();
        });

        // ---------- Subject -> Exercise (one-to-many) ----------
        // Prefers an explicit FK property Exercise.SubjectId (int)
        modelBuilder.Entity<Subject>()
            .HasMany(s => s.Exercises)
            .WithOne(e => e.Subject)
            .HasForeignKey(e => e.SubjectId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
