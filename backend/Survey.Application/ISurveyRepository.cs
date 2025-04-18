using Survey.Domain.Entities;

public interface ISurveyRepository
{
    Task<SurveyEntity> CreateAsync(SurveyEntity survey);
}