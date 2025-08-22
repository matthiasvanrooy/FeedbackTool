using System.Text.Json.Serialization;
using Feedbacktool;
using Feedbacktool.Models; // ToolContext
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ToolContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"),
        sql => sql.MigrationsAssembly("Feedbacktool.Api"))
);

builder.Services.AddControllers()    
    .AddJsonOptions(o => 
    {
    o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    o.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull; 
    });;
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ---- Seed test data (DEV only) ----
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ToolContext>();

    // Ensure DB is created (safe with Postgres migrations)
    db.Database.EnsureCreated();

    if (!db.ClassGroups.Any())
    {
        var cg1 = new ClassGroup { Name = "1KLTa" };
        var cg2 = new ClassGroup { Name = "2KLT" };
        db.ClassGroups.AddRange(cg1, cg2);

        var s1 = new Subject { Name = "Math" };
        var s2 = new Subject { Name = "English" };
        db.Subjects.AddRange(s1, s2);

        var alice = new User
        {
            Name = "Alice",
            Email = "alice@example.com",
            Password = "pw1", // NOTE: plain text for testing, hash in prod
            IsTeacher = false,
            ClassGroup = cg1,
            Subjects = new List<Subject> { s2 } // Alice takes English
        };

        var bob = new User
        {
            Name = "Bob",
            Email = "bob@example.com",
            Password = "pw2",
            IsTeacher = true,
            ClassGroup = cg2,
            Subjects = new List<Subject> { s1 } // Bob takes Math
        };

        db.Users.AddRange(alice, bob);

        db.Exercises.AddRange(
            new Exercise
            {
                Name = "Grammar test",
                Description = "Basic grammar quiz",
                Category = Category.Grammatica,
                Score = 10,
                UserScore = 0,
                Subject = s2
            },
            new Exercise
            {
                Name = "Math drill",
                Description = "Multiplication practice",
                Category = Category.Kennis,
                Score = 20,
                UserScore = 0,
                Subject = s1
            }
        );

        var sg1 = new ScoreGroup { Name = "Math Midterm", SubjectId = s1.Id };
        var sg2 = new ScoreGroup { Name = "English Oral Exam", SubjectId = s2.Id };

        // Add users to score groups (many-to-many)
        sg1.Users.Add(bob);   // Bob is in Math Midterm
        sg2.Users.Add(alice); // Alice is in English Oral Exam

        db.ScoreGroups.AddRange(sg1, sg2);

        db.SaveChanges();
    }
}

app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();
app.Run();