using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql;
using Survey.Infrastructure.Data;
using System.Threading;

namespace Survey.API.Data
{
    public static class DatabaseInitializer
    {
        public static void MigrateAndSeed(this IServiceProvider serviceProvider)
        {
            const int maxRetries = 10;
            const int delaySeconds = 3;

            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                try
                {
                    using var scope = serviceProvider.CreateScope();
                    var context = scope.ServiceProvider.GetRequiredService<SurveyDbContext>();
                    var logger = scope.ServiceProvider.GetRequiredService<ILogger<SurveyDbContext>>();

                    logger.LogInformation("Attempting to migrate database. Try {Attempt}/{Max}", attempt, maxRetries);
                    context.Database.Migrate();

                    if (!context.SurveyTypes.Any())
                    {
                        context.SurveyTypes.AddRange(
                            new() { SurveyTypeName = "Saved" },
                            new() { SurveyTypeName = "Completed" }
                        );
                    }

                    if (!context.UserTypes.Any())
                    {
                        context.UserTypes.AddRange(
                            new() { UserTypeName = "Superuser" },
                            new() { UserTypeName = "Experimenter" },
                            new() { UserTypeName = "Experimentee" }
                        );
                    }

                    if (!context.SurveyCompletionTypes.Any())
                    {
                        context.SurveyCompletionTypes.AddRange(
                            new() { SurveyCompletionTypeName = "Saved" },
                            new() { SurveyCompletionTypeName = "Completed" }
                        );
                    }

                    context.SaveChanges();
                    logger.LogInformation("Database migrated and seeded successfully.");
                    return;
                }
                catch (PostgresException ex) when (ex.SqlState == "57P03") // database is starting
                {
                    Console.WriteLine($"[WARN] Postgres not ready (attempt {attempt}/{maxRetries}). Waiting {delaySeconds}s...");
                    Thread.Sleep(TimeSpan.FromSeconds(delaySeconds));
                }
                catch (Exception ex) when (attempt < maxRetries)
                {
                    Console.WriteLine($"[ERROR] Migration failed: {ex.Message} (attempt {attempt}/{maxRetries}). Retrying...");
                    Thread.Sleep(TimeSpan.FromSeconds(delaySeconds));
                }
            }

            throw new Exception("Could not migrate and seed database: Postgres never became ready.");
        }
    }
}
