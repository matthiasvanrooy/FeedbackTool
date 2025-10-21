using Feedbacktool.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Feedbacktool.Api.Data;

public static class DbSeeder
{
    public static void SeedTestData(ToolContext db)
    {
        // --- Clear & recreate the database (DEV only) ---
        db.Database.EnsureDeleted();
        db.Database.Migrate();

        // --- ClassGroups ---
        var cg1 = new ClassGroup { Name = "1KLTa" };
        var cg2 = new ClassGroup { Name = "2KLT" };
        db.ClassGroups.AddRange(cg1, cg2);

        // --- Subjects ---
        var s1 = new Subject { Name = "Math" };
        var s2 = new Subject { Name = "English" };
        db.Subjects.AddRange(s1, s2);

        // --- Users ---
        var hasher = new PasswordHasher<User>();
        var admin = new User
        {
            Name = "Admin",
            Email = "admin@example.com",
            Role = Role.Admin,
            ClassGroup = cg1,
            Subjects = new List<Subject> { s2 }
        };
        admin.Password = hasher.HashPassword(admin, "admin");

        var alice = new User
        {
            Name = "Alice",
            Email = "alice@example.com",
            Role = Role.Teacher,
            ClassGroup = cg1,
            Subjects = new List<Subject> { s2 }
        };
        alice.Password = hasher.HashPassword(alice, "alice");

        var bob = new User
        {
            Name = "Bob",
            Email = "bob@example.com",
            Password = "pw2",
            Role = Role.Student,
            ClassGroup = cg2,
            Subjects = new List<Subject> { s1 }
        };
        bob.Password = hasher.HashPassword(bob, "bob");

        var charlie = new User
        {
            Name = "Charlie",
            Email = "charlie@example.com",
            Role = Role.Student,
            ClassGroup = cg2,
            Subjects = new List<Subject> { s1, s2 }
        };
        charlie.Password = hasher.HashPassword(charlie, "charlie");

        db.Users.AddRange(admin, alice, bob, charlie);
        db.SaveChanges(); // generate IDs

        // --- Exercises with Items ---
        var englishExercise = new Exercise
        {
            Name = "Present Simple",
            Description = "Practice with present simple tense",
            Category = Category.Grammatica,
            MaxScore = 30,
            SubjectId = s2.Id,
            Items =
            {
                new ExerciseItem { Question = "He ___ (play) football.", Answer = "plays" },
                new ExerciseItem { Question = "She ___ (not/eat) meat.", Answer = "does not eat" },
                new ExerciseItem { Question = "___ you ___ (like) pizza?", Answer = "Do you like" }
            }
        };

        var mathExercise = new Exercise
        {
            Name = "Math Basics",
            Description = "Simple arithmetic questions",
            Category = Category.Kennis,
            MaxScore = 20,
            SubjectId = s1.Id,
            Items =
            {
                new ExerciseItem { Question = "12 × 8 = ?", Answer = "96" },
                new ExerciseItem { Question = "Simplify 12/16", Answer = "3/4" },
                new ExerciseItem { Question = "15 + 27 = ?", Answer = "42" }
            }
        };

        db.Exercises.AddRange(englishExercise, mathExercise);
        db.SaveChanges(); // save exercises & items

        // --- ScoreGroups ---
        var sg1 = new ScoreGroup { Name = "Math Midterm", SubjectId = s1.Id };
        var sg2 = new ScoreGroup { Name = "English Oral Exam", SubjectId = s2.Id };

        sg1.Users.Add(bob);
        sg1.Users.Add(charlie);
        sg2.Users.Add(alice);
        sg2.Users.Add(charlie);

        db.ScoreGroups.AddRange(sg1, sg2);

        // --- Simulate student submissions (ScoreRecords) ---
        var students = new List<User> { bob, charlie };
        var exercises = new List<Exercise> { englishExercise, mathExercise };

        foreach (var student in students)
        {
            foreach (var exercise in exercises)
            {
                var itemResults = new List<ExerciseItemResult>();
                int correctCount = 0;

                foreach (var item in exercise.Items)
                {
                    // Randomize correct/incorrect answers
                    bool isCorrect = new Random().Next(0, 2) == 1; // 50% chance
                    string givenAnswer = isCorrect ? item.Answer! : "wrong answer";

                    if (isCorrect) correctCount++;

                    itemResults.Add(new ExerciseItemResult
                    {
                        ExerciseItemId = item.Id,
                        GivenAnswer = givenAnswer,
                        IsCorrect = isCorrect
                    });
                }

                db.ScoreRecords.Add(new ScoreRecord
                {
                    UserId = student.Id,
                    ExerciseId = exercise.Id,
                    Value = correctCount,
                    ItemResults = itemResults,
                    RecordedAt = DateTime.UtcNow
                });
            }
        }

        // --- FeedbackRules ---
        var feedbackRules = new List<FeedbackRule>
        {
            new FeedbackRule
            {
                ExerciseId = englishExercise.Id,
                Threshold = 0,
                FeedbackMessage = "You need more practice on Present Simple.",
                SuggestedExercises = new List<Exercise> { englishExercise }
            },
            new FeedbackRule
            {
                ExerciseId = englishExercise.Id,
                Threshold = 60,
                FeedbackMessage = "Good effort! Review a few tricky questions.",
                SuggestedExercises = new List<Exercise> { englishExercise }
            },
            new FeedbackRule
            {
                ExerciseId = englishExercise.Id,
                Threshold = 90,
                FeedbackMessage = "Excellent! You mastered the Present Simple.",
                SuggestedExercises = new List<Exercise>()
            },
            new FeedbackRule
            {
                ExerciseId = mathExercise.Id,
                Threshold = 0,
                FeedbackMessage = "Keep practicing basic arithmetic.",
                SuggestedExercises = new List<Exercise> { mathExercise }
            },
            new FeedbackRule
            {
                ExerciseId = mathExercise.Id,
                Threshold = 70,
                FeedbackMessage = "Well done! Try some additional challenges.",
                SuggestedExercises = new List<Exercise> { mathExercise }
            }
        };

        db.FeedbackRules.AddRange(feedbackRules);
        db.SaveChanges(); // save all ScoreRecords & FeedbackRules
    }
}
