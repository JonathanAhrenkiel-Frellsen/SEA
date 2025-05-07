// File: Survey.API/Data/DatabaseInitializer.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Survey.Infrastructure.Data;

namespace Survey.API.Data
{
    public static class DatabaseInitializer
    {
        public static void MigrateAndSeed(this IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<SurveyDbContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<SurveyDbContext>>();

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
            logger.LogInformation("Database migrated and seeded.");
        }
    }
}