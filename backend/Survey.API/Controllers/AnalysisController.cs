using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Survey.Application;
using Survey.Infrastructure.Data;

namespace Survey.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnalysisController : ControllerBase 
    {
        private readonly SurveyDbContext _context;

        public AnalysisController(SurveyDbContext context)
        {
            _context = context;
        }

        [Authorize]
        [HttpGet("surveyResponseOverTime/{surveyId}")]
        public async Task<ActionResult<List<SurveyResponseOverTimeDto>>> GetSurveyResponseOverTime(string surveyId)
        {
            if (!int.TryParse(surveyId, out int surveyIdInt))
            {
                return BadRequest("Invalid surveyId");
            }

            var completions = await _context.SurveyCompletion
                .Where(sc => sc.SurveyId == surveyIdInt)
                .GroupBy(sc => sc.SurveyCompletionDate.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    Count = g.Count()
                })
                .OrderBy(x => x.Date)
                .ToListAsync();

            return Ok(completions);
        }
        
        [Authorize]
        [HttpGet("surveyCompletionRate/{surveyId}")]
        public async Task<ActionResult<SurveyCompletionRateDto>> GetSurveyCompletionRate(string surveyId)
        {
            if (!int.TryParse(surveyId, out int surveyIdInt))
                return BadRequest("Invalid surveyId");

            var totalQuestions = await _context.Questionnaires
                .CountAsync(q => q.SurveyId == surveyIdInt);

            if (totalQuestions == 0)
                return BadRequest("No questions found for this survey.");

            var answers = await _context.SurveyAnswer
                .Where(sa => sa.SurveyId == surveyIdInt)
                .ToListAsync();

            var answersPerUser = answers
                .GroupBy(a => a.UserId)
                .Select(g => new { UserId = g.Key, AnswerCount = g.Count() })
                .ToList();

            var histogram = answersPerUser
                .GroupBy(u => u.AnswerCount)
                .Select(g => new
                {
                    AnsweredCount = g.Key,
                    UserCount = g.Count()
                })
                .OrderBy(h => h.AnsweredCount)
                .ToList();

            return Ok(new
            {
                TotalQuestions = totalQuestions,
                Histogram = histogram
            });
        }
        
        [Authorize]
        [HttpGet("surveyAnswers/{surveyId}")]
        public async Task<ActionResult<List<Dictionary<string, object>>>> GetSurveyAnswers(string surveyId, [FromQuery(Name = "page")] string pageNr)
        {
            const int PageSize = 20;

            if (!int.TryParse(surveyId, out int surveyIdInt))
                return BadRequest("Invalid surveyId");

            if (!int.TryParse(pageNr, out int page))
                return BadRequest("Invalid page number");

            if (page < 1)
                page = 1;

            var allAnswers = await _context.SurveyAnswer
                .Where(sa => sa.SurveyId == surveyIdInt)
                .Include(sa => sa.Questionnaire)
                .ToListAsync();

            var questionIds = allAnswers
                .Select(a => a.QuestionnaireId)
                .Distinct()
                .OrderBy(id => id)
                .ToList();

            var questionLabels = allAnswers
                .GroupBy(a => a.QuestionnaireId)
                .ToDictionary(g => g.Key, g => g.First().Questionnaire.QuestionnaireTitle ?? $"Q{g.Key}");

            var groupedByUser = allAnswers
                .GroupBy(a => a.UserId)
                .OrderBy(g => g.Key)
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            var response = new List<Dictionary<string, object>>();

            foreach (var group in groupedByUser)
            {
                var userAnswers = new Dictionary<string, object>
                {
                    ["UserId"] = group.Key
                };

                foreach (var qId in questionIds)
                {
                    var answer = group.FirstOrDefault(a => a.QuestionnaireId == qId)?.Answer ?? "";
                    var columnLabel = questionLabels[qId];
                    userAnswers[columnLabel] = answer;
                }

                response.Add(userAnswers);
            }

            return Ok(response);
        }
    }
}