namespace Feedbacktool;

using Microsoft.EntityFrameworkCore;
using Models;
using System.Collections.Generic;

public class ToolContext : DbContext
{
    public DbSet<Exercise> Exercises { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Subject> Subjects { get; set; }
    public DbSet<ClassGroup> ClassGroups { get; set; }
    public DbSet<ScoreGroup> ScoreGroups { get; set; }

    public ToolContext(DbContextOptions<ToolContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ---------- User <-> Subject (many-to-many) ----------
        modelBuilder.Entity<User>()
            .HasMany(u => u.Subjects)
            .WithMany(s => s.Users) // If Subject doesn't have Users, change to .WithMany()
            .UsingEntity<Dictionary<string, object>>(
                "UserSubject",
                j => j.HasOne<Subject>().WithMany().HasForeignKey("SubjectId")
                      .OnDelete(DeleteBehavior.Cascade),
                j => j.HasOne<User>().WithMany().HasForeignKey("UserId")
                      .OnDelete(DeleteBehavior.Cascade),
                j =>
                {
                    j.HasKey("UserId", "SubjectId");
                    j.ToTable("UserSubjects");
                });

        // ---------- User <-> ScoreGroup (many-to-many) ----------
        modelBuilder.Entity<User>()
            .HasMany(u => u.ScoreGroups)
            .WithMany() // If ScoreGroup has Users collection, replace with .WithMany(sg => sg.Users)
            .UsingEntity<Dictionary<string, object>>(
                "UserScoreGroup",
                j => j.HasOne<ScoreGroup>().WithMany().HasForeignKey("ScoreGroupId")
                      .OnDelete(DeleteBehavior.Cascade),
                j => j.HasOne<User>().WithMany().HasForeignKey("UserId")
                      .OnDelete(DeleteBehavior.Cascade),
                j =>
                {
                    j.HasKey("UserId", "ScoreGroupId");
                    j.ToTable("UserScoreGroups");
                });

        // ---------- Subject -> Exercise (one-to-many) ----------
        modelBuilder.Entity<Subject>()
            .HasMany(s => s.Exercises)
            .WithOne(e => e.Subject)
            .HasForeignKey("SubjectId")
            .OnDelete(DeleteBehavior.Cascade);

        // ---------- ScoreGroup -> Subject (many-to-one) ----------
        modelBuilder.Entity<ScoreGroup>()
            .HasOne(sg => sg.Subject)
            .WithMany() // If Subject has ICollection<ScoreGroup> ScoreGroups, use .WithMany(s => s.ScoreGroups)
            .HasForeignKey("SubjectId")
            .OnDelete(DeleteBehavior.Cascade);

        // ---------- User -> ClassGroup (many-to-one) ----------
        modelBuilder.Entity<User>()
            .HasOne(u => u.ClassGroup)
            .WithMany(cg => cg.Users) // If ClassGroup lacks Users, change to .WithMany()
            .HasForeignKey("ClassGroupId") // shadow FK if you don't add the property on User
            .OnDelete(DeleteBehavior.Restrict);

        // ---------- Indexes / constraints ----------
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();
    }
}
