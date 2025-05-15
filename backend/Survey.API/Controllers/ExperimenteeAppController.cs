using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Survey.API.Attributes;
using Survey.Application;
using Survey.Domain.Entities;
using SurveyDbContext = Survey.Infrastructure.Data.SurveyDbContext;

namespace Survey.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExperimenteeAppController : ControllerBase
    {
        private readonly SurveyDbContext _context;

        public ExperimenteeAppController(SurveyDbContext context)
        {
            _context = context;
        }

        // POST: api/ExperimenteeApp/GetListofSavedSurveys
        [HttpPost("GetListofSavedSurveys")]
        public async Task<IActionResult> GetListofSavedSurveys([FromBody] UserDto user)
        {
            var savedSurveys = await _context.SurveyCompletion
                .Where(s => s.UserId == user.UserId && s.SurveyCompletionTypeId == 1)
                .ToListAsync();

            if (savedSurveys == null || savedSurveys.Count == 0)
            {
                return NotFound("No saved surveys found for the given user.");
            }

            var surveyIds = savedSurveys.Select(s => s.SurveyId).Distinct().ToList();

            var surveys = await _context.Surveys
                .Where(s => surveyIds.Contains(s.SurveyId) && s.EndDate <= DateTime.Now)
                .ToListAsync();

            return Ok(surveys);
        }

        // POST: api/ExperimenteeApp/GetListofNewSurveys
        [HttpPost("GetListofNewSurveys")]
        public async Task<IActionResult> GetListofNewSurveys([FromBody] UserDto user)
        {
            var savedSurveys = await _context.SurveyCompletion
                .Where(s => s.UserId == user.UserId)
                .ToListAsync();

            var surveyIds = savedSurveys.Select(s => s.SurveyId).Distinct().ToList();

            var surveys = await _context.Surveys
                .Where(s => !surveyIds.Contains(s.SurveyId) && s.SurveyTypeId != 2 && s.EndDate <= DateTime.Now)
                .ToListAsync();

            return Ok(surveys);
        }

        // POST: api/ExperimenteeApp/GetListofCompletedSurveys
        [HttpPost("GetListofCompletedSurveys")]
        public async Task<IActionResult> GetListofCompletedSurveys([FromBody] UserDto user)
        {
            var completedSurveys = await _context.SurveyCompletion
                .Where(s => s.UserId == user.UserId && s.SurveyCompletionTypeId == 2)
                .ToListAsync();
            if (completedSurveys == null || completedSurveys.Count == 0)
            {
                return NotFound("No completed surveys found for the given user.");
            }
            var surveyIds = completedSurveys.Select(s => s.SurveyId).Distinct().ToList();
            var surveys = await _context.Surveys
                .Where(s => surveyIds.Contains(s.SurveyId) && s.EndDate <= DateTime.Now)
                .ToListAsync();
            return Ok(surveys);
        }
        
        [Authorize]
        [Pincode] // needs the surveyId form the HttpGET!
        [HttpGet("LoadSurvey/{surveyId}")]
        public async Task<IActionResult> LoadSurvey(string surveyId)
        {
            var userId = User.FindFirst("UserId")!.Value;

            var survey = await _context.Surveys
                .Include(s => s.Questionnaires)
                    .ThenInclude(q => q.MultipleChoices)
                .FirstOrDefaultAsync(s => s.SurveyId.ToString() == surveyId);

            if (survey == null)
            {
                return NotFound("Survey not found.");
            }


            var Data = new ExperimenteeAppDto
            {
                SurveyId = survey.SurveyId,
                SurveyTitle = survey.SurveyTitle,
                SurveyDescription = survey.SurveyDescription,
                UserId = int.Parse(userId),
                SurveyStoredAnwsers = [.. survey.Questionnaires.Select(q => new SurveyStoredAnwsersDto
                {
                    QuestionnaireId = q.QuestionnaireId,
                    QuestionnaireTitle = q.QuestionnaireTitle,
                    QuestionnairePos = q.QuestionnairePos,
                    InputType = q.InputType,
                    Range = q.Range,
                    SurveyAnswer = string.Empty, // Initialize with empty answer
                    MultipleChoices = q.MultipleChoices != null
                        ? [.. q.MultipleChoices.Select(mc => new MultipleChoicesDto
                        {
                            MultipleChoiceId = mc.MultipleChoiceId,
                            MultipleChoiceName = mc.MultipleChoiceName
                        })]
                        : new List<MultipleChoicesDto>()
                })]
            };

            var savedAnswers = await _context.SurveyAnswer
                .Where(sa => sa.SurveyCompletion != null && sa.SurveyCompletion.UserId.ToString() == userId
                && sa.SurveyCompletion.SurveyId.ToString() == surveyId)
                .ToListAsync();

            foreach (var answer in savedAnswers)
            {
                var storedAnswer = Data.SurveyStoredAnwsers.FirstOrDefault(s => s.QuestionnaireId == answer.QuestionnaireId);
                if (storedAnswer != null)
                {
                    storedAnswer.SurveyAnswer = answer.Answer;
                }
            }

            return Ok(Data);
        }


        // POST: api/ExperimenteeApp/SaveSurveyAnswer
        [Authorize]
        [HttpPost("SaveSurveyAnswer")]
        public async Task<IActionResult> SaveSurveyAnswers([FromBody] SurveySaveAnswerDto newSurveyAnswer)
        {
            var userId = User.FindFirst("UserId")!.Value;

            if (newSurveyAnswer == null)
            {
                return BadRequest("No survey answer data was passed");
            }

            var existingData = await _context.SurveyAnswer
                .Include(sa => sa.SurveyCompletion)
                .FirstOrDefaultAsync(s => s.QuestionnaireId == newSurveyAnswer.QuestionnaireId
                    && s.SurveyCompletion != null
                    && s.SurveyCompletion.UserId.ToString() == userId
                    && s.SurveyCompletion.SurveyId == newSurveyAnswer.SurveyId);

            if (existingData == null)
            {

                var newData = new SurveyCompletion
                {
                    SurveyId = newSurveyAnswer.SurveyId,
                    UserId = int.Parse(userId),
                    SurveyCompletionDate = DateTime.UtcNow,
                    SurveyCompletionTypeId = 1, // 1 is for saved surveys

                    SurveyAnswers =
                    [
                        new SurveyAnswer
                        {
                            QuestionnaireId = newSurveyAnswer.QuestionnaireId,
                            Answer = newSurveyAnswer.SurveyAnswer ?? string.Empty
                        }
                    ]
                };
                _context.SurveyCompletion.Add(newData);
            }
            else
            {
                existingData.Answer = newSurveyAnswer.SurveyAnswer ?? string.Empty;
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest("Error while saving survey answers.");
            }

            return Ok("Survey answer saved successfully.");
        }

        // POST: api/ExperimenteeApp/CompleteSurvey
        [Authorize]
        [HttpGet("CompleteSurvey/{surveyId}")]
        public async Task<IActionResult> CompleteSurvey(string surveyId)
        {
            var userId = User.FindFirst("UserId")!.Value;
            
            Console.WriteLine($"surveyId: {surveyId}");
            
            var existingData = await _context.SurveyCompletion
                .FirstOrDefaultAsync(s => s.UserId.ToString() == userId
                && s.SurveyId.ToString() == surveyId);
            if (existingData == null)
            {
                return NotFound("Survey not found.");
            }
            existingData.SurveyCompletionTypeId = 2; // 2 is for completed surveys
            existingData.SurveyCompletionDate = DateTime.UtcNow;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest("Error while completing the survey.");
            }
            return Ok("Survey completed successfully.");
        }

        // POST: api/ExperimenteeApp/DeleteSavedSurvey
        [Authorize]
        [HttpDelete("DeleteSavedSurvey{surveyId}")]
        public async Task<IActionResult> DeleteSavedSurvey(string surveyId)
        {
            var userId = User.FindFirst("UserId")!.Value;

            var survey = await _context.Surveys
                .FirstOrDefaultAsync(s => s.SurveyId.ToString() == surveyId);

            if (survey != null && survey.EndDate <= DateTime.Now)
            {
                return BadRequest("Cannot delete a survey that has already ended.");
            }

            var existingData = await _context.SurveyCompletion
                .Include(s => s.SurveyAnswers)
                .FirstOrDefaultAsync(s => s.UserId.ToString() == userId
                && s.SurveyId.ToString() == surveyId
                && s.SurveyCompletionTypeId == 1);

            if (existingData == null)
            {
                return NotFound("Survey not found.");
            }

            _context.SurveyCompletion.Remove(existingData);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest("Error while deleting the saved survey.");
            }
            return Ok("Saved survey deleted successfully.");
        }
    }
}
