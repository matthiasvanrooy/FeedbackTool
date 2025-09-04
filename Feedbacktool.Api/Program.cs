using System.Text.Json.Serialization;
using Feedbacktool.Models;                 // ToolContext
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Feedbacktool;
using Feedbacktool.Api.AutoMapper;
using Feedbacktool.Api.Services; // MappingProfile

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
        cfg.AddProfile<MappingProfile>(); // your profile
    }, loggerFactory); // <- satisfy the 2-arg ctor

    return config.CreateMapper();
});

var app = builder.Build();

// ---- Seed test data (DEV only) ----
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ToolContext>();

    // In dev, EnsureCreated is OK; for migrations, prefer db.Database.Migrate()
    if (app.Environment.IsDevelopment())
    {
       db.Database.Migrate(); 
    }

    if (!db.ClassGroups.Any())
    {
        var cg1 = new ClassGroup { Name = "1KLTa" };
        var cg2 = new ClassGroup { Name = "2KLT" };
        db.ClassGroups.AddRange(cg1, cg2);

        var s1 = new Subject { Name = "Math" };
        var s2 = new Subject { Name = "English" };
        db.Subjects.AddRange(s1, s2);

        var admin = new User
        {
            Name = "Admin",
            Email = "admin@example.com",
            Password = "admin", // dev only; hash in prod
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

        var sg1 = new ScoreGroup { Name = "Math Midterm", Subject = s1};
        var sg2 = new ScoreGroup { Name = "English Oral Exam", Subject = s2 };

        // many-to-many
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
