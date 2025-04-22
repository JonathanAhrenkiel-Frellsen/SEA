using Survey.Domain.Entities;
using Survey.Infrastructure.Data;

public class SurveyRepository : ISurveyRepository
{
    private readonly SurveyDbContext _context;

    public SurveyRepository(SurveyDbContext context)
    {
        _context = context;
    }

    public async Task<SurveyEntity> CreateAsync(SurveyEntity survey)
    {
        _context.Surveys_Ignore.Add(survey);
        await _context.SaveChangesAsync();
        return survey;
    }
}
