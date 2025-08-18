using Feedbacktool;                 // <- ToolContext lives here
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Choose your provider/connection string
builder.Services.AddDbContext<ToolContext>(opt =>
        opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
            // keep migrations in the API project:
            .UseApplicationServiceProvider(builder.Services.BuildServiceProvider())  // safe if needed
            .EnableSensitiveDataLogging(builder.Environment.IsDevelopment())
            .EnableDetailedErrors(builder.Environment.IsDevelopment())
            .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
            .UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
                sql => sql.MigrationsAssembly("Feedbacktool.Api")) // <— migrations go into API
);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();
app.Run();