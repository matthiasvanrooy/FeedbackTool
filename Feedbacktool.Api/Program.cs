using System.Text;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Feedbacktool;
using Feedbacktool.Api.AutoMapper;
using Feedbacktool.Api.Data;
using Feedbacktool.Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

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

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        var jwt = builder.Configuration.GetSection("Jwt");
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwt["Issuer"],
            ValidAudience = jwt["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!))
        };

        // Read token from cookie
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                if (context.Request.Cookies.ContainsKey("jwt"))
                {
                    context.Token = context.Request.Cookies["jwt"];
                }
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ClassGroupService>();
builder.Services.AddScoped<ScoreGroupService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<ExerciseService>();
builder.Services.AddScoped<SubjectService>();
builder.Services.AddScoped<ILoginService, LoginService>();
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
