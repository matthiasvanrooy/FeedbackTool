namespace Feedbacktool;

using Microsoft.EntityFrameworkCore;
using Feedbacktool.Models; // use full namespace
using System.Collections.Generic;

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
        
        // ---- Table-per-Type (TPT) mapping for inheritance ----
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
            .OnDelete(DeleteBehavior.Restrict);
        
        // ---------- User <-> ScoreGroup (many-to-many) ----------
        // If you added ScoreGroup.Users, use WithMany(sg => sg.Users); else keep WithMany()
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
        
        // ---------- Indexes / constraints ----------
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();
        
        modelBuilder.Entity<User>().Property(u => u.Role).HasConversion<string>();

        // ---------- Subject -> Exercise (one-to-many) ----------
        // Prefer a real FK property Exercise.SubjectId (int) instead of shadow FK.
        modelBuilder.Entity<Subject>()
            .HasMany(s => s.Exercises)
            .WithOne(e => e.Subject)
            .HasForeignKey(e => e.SubjectId)
            .OnDelete(DeleteBehavior.Cascade);

        // ---------- ScoreGroup -> Subject (many-to-one) ----------
        modelBuilder.Entity<ScoreGroup>()
            .HasOne(sg => sg.Subject)
            .WithMany(s => s.ScoreGroups) // add Subject.ScoreGroups collection to your model for this
            .HasForeignKey(sg => sg.SubjectId)
            .OnDelete(DeleteBehavior.Cascade);
        

    }
}
