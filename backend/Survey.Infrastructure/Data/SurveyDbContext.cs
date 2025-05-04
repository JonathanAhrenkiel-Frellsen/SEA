using Microsoft.EntityFrameworkCore;
using Survey.Domain.Entities;

namespace Survey.Infrastructure.Data;

public class SurveyDbContext : DbContext
{
    public SurveyDbContext(DbContextOptions<SurveyDbContext> options) : base(options) { }
    public DbSet<Questionnaire> Questionnaires => Set<Questionnaire>();
    public DbSet<SurveyAnswer> SurveyAnswer => Set<SurveyAnswer>();
    public DbSet<User> Users => Set<User>();
    public DbSet<UserType> UserTypes => Set<UserType>();
    public DbSet<SurveyCompletion> SurveyCompletion => Set<SurveyCompletion>();
    public DbSet<DesignedSurvey> Surveys => Set<DesignedSurvey>();
    public DbSet<SurveyType> SurveyTypes => Set<SurveyType>();
    //public DbSet<SurveyPrivateKey> SurveyPrivateKeys => Set<SurveyPrivateKey>();    

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //modelBuilder.Entity<SurveyAnswer>()
        //    .HasOne(sc => sc.Questionnaire)
        //    .WithMany()
        //    .HasForeignKey(sc => sc.QuestionnaireId)
        //    .OnDelete(DeleteBehavior.Restrict);

        //modelBuilder.Entity<SurveyAnswer>()
        //    .HasOne(sc => sc.Survey)
        //    .WithMany()
        //    .HasForeignKey(sc => sc.SurveyId)
        //    .OnDelete(DeleteBehavior.Restrict);

        //modelBuilder.Entity<SurveyCompletion>()
        //    .HasOne(sc => sc.Survey)
        //    .WithMany()
        //    .HasForeignKey(sc => sc.SurveyId)
        //    .OnDelete(DeleteBehavior.Restrict);

        //modelBuilder.Entity<SurveyCompletion>()
        //    .HasOne(sc => sc.User)
        //    .WithMany()
        //    .HasForeignKey(sc => sc.UserId)
        //    .OnDelete(DeleteBehavior.Restrict);

        //modelBuilder.Entity<SurveyAnswer>()
        //   .HasOne(sa => sa.SurveyCompletion)
        //   .WithMany(sc => sc.SurveyAnswers)
        //   .HasForeignKey(sa => sa.SurveyCompletionId)
        //   .OnDelete(DeleteBehavior.Restrict);

        //modelBuilder.Entity<SurveyCompletion>()
        //    .HasOne(sc => sc.Survey)
        //    .WithMany()
        //    .HasForeignKey(sc => sc.SurveyId)
        //    .OnDelete(DeleteBehavior.Restrict);

        
        modelBuilder.Entity<Questionnaire>()
            .HasOne(q => q.Survey)
            .WithMany(s => s.Questionnaires)
            .HasForeignKey(q => q.SurveyId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<SurveyCompletion>()
           .HasOne(sc => sc.User)
           .WithMany(u => u.SurveyCompletions)
           .HasForeignKey(sc => sc.UserId)
           .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<DesignedSurvey>()
            .HasOne(s => s.User)
            .WithMany(u => u.Surveys)
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<SurveyAnswer>()
            .HasOne(sa => sa.SurveyCompletion)
            .WithMany(sc => sc.SurveyAnswers)
            .HasForeignKey(sa => sa.SurveyCompletionId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<SurveyAnswer>()
            .HasOne(sa => sa.Questionnaire)
            .WithMany(q => q.SurveyAnswers)
            .HasForeignKey(sa => sa.QuestionnaireId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<SurveyCompletion>()
            .HasOne(sc => sc.Survey)
            .WithMany(s => s.SurveyCompletions)
            .HasForeignKey(sc => sc.SurveyId)
            .OnDelete(DeleteBehavior.Cascade);
    }
    //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //{
    //    optionsBuilder.UseLazyLoadingProxies();
    //}

}
