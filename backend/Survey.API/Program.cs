using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Survey.Application;
using Survey.Infrastructure.Data;
using SurveyDbContext = Survey.Infrastructure.Data.SurveyDbContext;

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
//builder.Services.AddDbContext<SurveyDbContext>(options =>
// options.UseSqlServer(connectionString));


var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
if (jwtSettings == null)
{
    throw new InvalidOperationException("JwtSettings configuration is missing or invalid.");
}
builder.Services.AddSingleton<JwtSettings>(jwtSettings);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwtSecret = builder.Configuration["JwtSettings:Secret"];
        if (string.IsNullOrEmpty(jwtSecret))
        {
            throw new InvalidOperationException("JwtSettings:Secret is not configured in the application settings.");
        }

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
        };
    });

builder.Services.AddAuthorization();


var app = builder.Build();

// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI();
// }

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.MapGet("/test-db", async (SurveyDbContext db) =>
{
    var count = await db.UserTypes.CountAsync();
    return Results.Ok($"Survey table contains {count} entries.");
});




app.Run();
