using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Survey.Infrastructure.Data
{
    public class SurveyDbContextFactory : IDesignTimeDbContextFactory<SurveyDbContext>
    {
        public SurveyDbContext CreateDbContext(string[] args)
        {
            // Find appsettings.json from Survey.API project
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../Survey.API"))
                .AddJsonFile("appsettings.json")
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<SurveyDbContext>();
            optionsBuilder.UseNpgsql(configuration.GetConnectionString("DefaultConnection")); // If you use PostgreSQL

            return new SurveyDbContext(optionsBuilder.Options);
        }
    }
}