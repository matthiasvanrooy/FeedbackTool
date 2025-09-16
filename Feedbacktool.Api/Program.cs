using System.Text.Json.Serialization;
using Feedbacktool.Models;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Feedbacktool;
using Feedbacktool.Api.AutoMapper;
using Feedbacktool.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// DbContext
builder.Services.AddDbContext<ToolContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"),
        sql => sql.MigrationsAssembly("Feedbacktool.Api"))
);

builder.Services.AddLogging();

// Controllers + JSON
builder.Services.AddControllers()
    .AddJsonOptions(o =>
    {
        o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        o.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ClassGroupService>();
builder.Services.AddScoped<ScoreGroupService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<ExerciseService>();
builder.Services.AddScoped<SubjectService>();
builder.Services.AddSingleton<IMapper>(sp =>
{
    var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
    var config = new AutoMapper.MapperConfiguration(cfg =>
    {
        cfg.AddProfile<MappingProfile>();
    }, loggerFactory);
    return config.CreateMapper();
});

var app = builder.Build();

// ---- Seed test data (DEV only) ----
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ToolContext>();

    if (app.Environment.IsDevelopment())
    {
       db.Database.Migrate(); 
    }

    if (!db.ClassGroups.Any())
    {
        // --- ClassGroups ---
        var cg1 = new ClassGroup { Name = "1KLTa" };
        var cg2 = new ClassGroup { Name = "2KLT" };
        db.ClassGroups.AddRange(cg1, cg2);

        // --- Subjects ---
        var s1 = new Subject { Name = "Math" };
        var s2 = new Subject { Name = "English" };
        db.Subjects.AddRange(s1, s2);

        // --- Users ---
        var admin = new User
        {
            Name = "Admin",
            Email = "admin@example.com",
            Password = "admin",
            Role = Role.Admin,
            ClassGroup = cg1,
            Subjects = new List<Subject> { s2 }
        };

        var alice = new User
        {
            Name = "Alice",
            Email = "alice@example.com",
            Password = "pw1",
            Role = Role.Teacher,
            ClassGroup = cg1,
            Subjects = new List<Subject> { s2 }
        };

        var bob = new User
        {
            Name = "Bob",
            Email = "bob@example.com",
            Password = "pw2",
            Role = Role.Student,
            ClassGroup = cg2,
            Subjects = new List<Subject> { s1 }
        };

        db.Users.AddRange(admin, alice, bob);

        // Save changes to generate IDs
        db.SaveChanges();

        // --- Exercises ---
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
        db.SaveChanges(); // save Exercises & ExerciseItems

        // --- ScoreRecords (sample for Bob) ---
        var bobRecord = new ScoreRecord
        {
            UserId = bob.Id,
            ExerciseId = englishExercise.Id,
            Value = 2, // 2 correct out of 3
            ItemResults = new List<ExerciseItemResult>
            {
                new ExerciseItemResult
                {
                    ExerciseItemId = englishExercise.Items.ElementAt(0).Id,
                    GivenAnswer = "play",
                    IsCorrect = false
                },
                new ExerciseItemResult
                {
                    ExerciseItemId = englishExercise.Items.ElementAt(1).Id,
                    GivenAnswer = "does not eat",
                    IsCorrect = true
                },
                new ExerciseItemResult
                {
                    ExerciseItemId = englishExercise.Items.ElementAt(2).Id,
                    GivenAnswer = "Do you like",
                    IsCorrect = true
                }
            }
        };

        db.ScoreRecords.Add(bobRecord);

        // --- ScoreGroups ---
        var sg1 = new ScoreGroup { Name = "Math Midterm", SubjectId = s1.Id };
        var sg2 = new ScoreGroup { Name = "English Oral Exam", SubjectId = s2.Id };

        // Many-to-many assignments
        sg1.Users.Add(bob);
        sg2.Users.Add(alice);

        db.ScoreGroups.AddRange(sg1, sg2);

        db.SaveChanges();
    }
}

app.UseSwagger();
app.UseSwaggerUI();
// app.UseHttpsRedirection(); // optional
app.MapControllers();
app.Run();
