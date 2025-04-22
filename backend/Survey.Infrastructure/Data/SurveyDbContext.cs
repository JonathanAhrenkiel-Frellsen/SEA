using Microsoft.EntityFrameworkCore;
using Survey.Domain.Entities;

namespace Survey.Infrastructure.Data;

public class SurveyDbContext : DbContext
{
    public SurveyDbContext(DbContextOptions<SurveyDbContext> options) : base(options) { }

    public DbSet<SurveyEntity> Surveys_Ignore => Set<SurveyEntity>();
    public DbSet<Questionnaire> Questionnaires => Set<Questionnaire>();
    public DbSet<SurveyAnswer> SurveyAnswer => Set<SurveyAnswer>();
    public DbSet<User> Users => Set<User>();
    public DbSet<UserType> UserTypes => Set<UserType>();
    public DbSet<SurveyCompletion> SurveyCompletion => Set<SurveyCompletion>();
    public DbSet<DesignedSurvey> Surveys => Set<DesignedSurvey>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SurveyAnswer>()
            .HasOne(sc => sc.Questionnaire)
            .WithMany()
            .HasForeignKey(sc => sc.QuestionnaireId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<SurveyAnswer>()
            .HasOne(sc => sc.SurveyCompletion)
            .WithMany()
            .HasForeignKey(sc => sc.SurveyCompletionId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<SurveyAnswer>()
            .HasOne(sc => sc.Survey)
            .WithMany()
            .HasForeignKey(sc => sc.SurveyId)
            .OnDelete(DeleteBehavior.Restrict);
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseLazyLoadingProxies();
    }

}
