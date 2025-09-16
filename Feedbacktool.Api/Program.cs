using System.Text.Json.Serialization;
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

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ToolContext>();

    if (app.Environment.IsDevelopment())
    {
        DbSeeder.SeedTestData(db);
    }
}

app.UseSwagger();
app.UseSwaggerUI();
// app.UseHttpsRedirection(); // optional
app.MapControllers();
app.Run();
