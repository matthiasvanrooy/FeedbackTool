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
    public DbSet<ScoreRecord> ScoreRecords { get; set; } = null!;

    public ToolContext(DbContextOptions<ToolContext> options) : base(options) { }

 protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ---- Table-per-Type (TPT) mapping ----
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
            .WithMany(s => s.ScoreGroups)
            .HasForeignKey(sg => sg.SubjectId)
            .OnDelete(DeleteBehavior.Cascade);

        // ---------- Subject -> Exercise (one-to-many) ----------
        modelBuilder.Entity<Subject>()
            .HasMany(s => s.Exercises)
            .WithOne(e => e.Subject)
            .HasForeignKey(e => e.SubjectId)
            .OnDelete(DeleteBehavior.Cascade);

        // ---------- User -> ScoreRecord (one-to-many) ----------
        modelBuilder.Entity<ScoreRecord>()
            .HasOne(sr => sr.User)
            .WithMany(u => u.ScoreRecords)
            .HasForeignKey(sr => sr.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // ---------- Exercise -> ScoreRecord (one-to-many) ----------
        modelBuilder.Entity<ScoreRecord>()
            .HasOne(sr => sr.Exercise)
            .WithMany(e => e.ScoreRecords)
            .HasForeignKey(sr => sr.ExerciseId)
            .OnDelete(DeleteBehavior.Cascade);

        // ---------- Unique constraint: one score per (User, Exercise) ----------
        modelBuilder.Entity<ScoreRecord>()
            .HasIndex(sr => new { sr.UserId, sr.ExerciseId })
            .IsUnique();

        // ---------- Indexes / constraints ----------
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<User>().Property(u => u.Role).HasConversion<int>();

        modelBuilder.Entity<User>(e =>
        {
            e.Property(x => x.Name).IsRequired().HasMaxLength(200);
            e.Property(x => x.Email).IsRequired().HasMaxLength(320);
            e.Property(x => x.Password).IsRequired();
        });

        modelBuilder.Entity<ClassGroup>(e =>
        {
            e.Property(x => x.Name).IsRequired().HasMaxLength(200);
        });

        modelBuilder.Entity<ScoreGroup>(e =>
        {
            e.Property(x => x.Name).IsRequired().HasMaxLength(200);
            e.HasIndex(x => new { x.SubjectId, x.Name }).IsUnique();
        });
        
        modelBuilder.Entity<Exercise>()
            .HasMany(e => e.Items)
            .WithOne(i => i.Exercise)
            .HasForeignKey(i => i.ExerciseId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<ExerciseItemResult>()
            .HasOne(r => r.ExerciseItem)
            .WithMany()
            .HasForeignKey(r => r.ExerciseItemId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ExerciseItemResult>()
            .HasOne(r => r.ScoreRecord)
            .WithMany(sr => sr.ItemResults)
            .HasForeignKey(r => r.ScoreRecordId)
            .OnDelete(DeleteBehavior.Cascade);

    }
}
