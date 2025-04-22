using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Survey.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

Console.WriteLine("Connection string: " + connectionString);

// Ensure the Microsoft.EntityFrameworkCore.SqlServer package is installed in your project
// You can install it using the following command in the terminal:
// dotnet add package Microsoft.EntityFrameworkCore.SqlServer

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
    var count = await db.Surveys_Ignore.CountAsync();
    return Results.Ok($"Survey table contains {count} entries.");
});

app.Run();
