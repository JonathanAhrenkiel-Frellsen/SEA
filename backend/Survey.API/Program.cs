using Microsoft.EntityFrameworkCore;
using Survey.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("Default");

Console.WriteLine("Connection string: " + connectionString);

builder.Services.AddDbContext<SurveyDbContext>(options =>
    options.UseNpgsql(connectionString));

var app = builder.Build();

// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI();
// }

app.UseAuthorization();
app.MapControllers();

app.MapGet("/test-db", async (SurveyDbContext db) =>
{
    var count = await db.Surveys.CountAsync();
    return Results.Ok($"Survey table contains {count} entries.");
});

app.Run();
