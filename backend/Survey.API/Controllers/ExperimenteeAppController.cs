using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        // POST: api/ExperimenteeApp/GetSurveybyPrivateKey
        [HttpPost("GetSurveybyPrivateKey")]
        public async Task<IActionResult> GetSurveybyPIN([FromBody] DesignedSurveyDto survey)
        {
            if (survey == null || string.IsNullOrEmpty(survey.PrivateKey) || survey.UserId <= 0)
            {
                return NotFound("Invalid Private Key.");
            }

            var surveyData = await _context.Surveys
                .FirstOrDefaultAsync(s => s.PrivateKey == survey.PrivateKey);

            if (surveyData == null)
            {
                return NotFound("Invalid Private Key.");
            }

            var data = new ExperimenteeAppDto
            {
                SurveyId = surveyData.SurveyId,
                UserId = survey.UserId
            };

            return Ok(LoadSurvey(data));

        }


        // POST: api/ExperimenteeApp/LoadSurvey
        [HttpPost("LoadSurvey")]
        public async Task<IActionResult> LoadSurvey([FromBody] ExperimenteeAppDto surveyLoadAnswer)
        {
            if (surveyLoadAnswer == null || surveyLoadAnswer.SurveyId <= 0 || surveyLoadAnswer.UserId <= 0)
            {
                return BadRequest("Invalid survey data.");
            }

            var survey = await _context.Surveys
                .Include(s => s.Questionnaires)
                    .ThenInclude(q => q.MultipleChoices)
                .FirstOrDefaultAsync(s => s.SurveyId == surveyLoadAnswer.SurveyId);

            if (survey == null)
            {
                return NotFound("Survey not found.");
            }


            var Data = new ExperimenteeAppDto
            {
                SurveyId = survey.SurveyId,
                SurveyTitle = survey.SurveyTitle,
                SurveyDescription = survey.SurveyDescription,
                UserId = surveyLoadAnswer.UserId,
                SurveyStoredAnwsers = [.. survey.Questionnaires.Select(q => new SurveyStoredAnwsersDto
                {
                    QuestionnaireId = q.QuestionnaireId,
                    QuestionnaireTitle = q.QuestionnaireTitle,
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


            // Fix for CS8602: Dereference of a possibly null reference.
            var savedAnswers = await _context.SurveyAnswer
                .Where(sa => sa.SurveyCompletion != null && sa.SurveyCompletion.UserId == surveyLoadAnswer.UserId
                && sa.SurveyCompletion.SurveyId == surveyLoadAnswer.SurveyId)
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
        [HttpPost("SaveSurveyAnswer")]
        public async Task<IActionResult> SaveSurveyAnswers([FromBody] SurveySaveAnswerDto newsurveyAnswer)
        {
            if (newsurveyAnswer == null)
            {
                return BadRequest("No survey answer data was passed");
            }

            if (!newsurveyAnswer.SurveyId.HasValue || !newsurveyAnswer.UserId.HasValue || !newsurveyAnswer.QuestionnaireId.HasValue)
            {
                return BadRequest("SurveyId, UserId, or QuestionnaireId is missing.");
            }

            var existingData = await _context.SurveyAnswer
                .Include(sa => sa.SurveyCompletion)
                .FirstOrDefaultAsync(s => s.QuestionnaireId == newsurveyAnswer.QuestionnaireId.Value
                    && s.SurveyCompletion != null
                    && s.SurveyCompletion.UserId == newsurveyAnswer.UserId.Value
                    && s.SurveyCompletion.SurveyId == newsurveyAnswer.SurveyId.Value);

            if (existingData == null)
            {

                var newData = new SurveyCompletion
                {
                    SurveyCompletionId = 0,
                    SurveyId = newsurveyAnswer.SurveyId.Value,
                    UserId = newsurveyAnswer.UserId.Value,
                    SurveyCompletionDate = DateTime.Now,
                    SurveyCompletionTypeId = 1, // 1 is for saved surveys

                    SurveyAnswers =
                    [
                        new SurveyAnswer
                        {
                            SurveyAnswerId = 0,
                            QuestionnaireId = newsurveyAnswer.QuestionnaireId.Value,
                            Answer = newsurveyAnswer.SurveyAnswer ?? string.Empty
                        }
                    ]
                };
                _context.SurveyCompletion.Add(newData);
            }
            else
            {
                existingData.Answer = newsurveyAnswer.SurveyAnswer ?? string.Empty; 
                //_context.Entry(existingData).State = EntityState.Modified;
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
        [HttpPost("CompleteSurvey")]
        public async Task<IActionResult> CompleteSurvey([FromBody] SurveySaveAnswerDto newsurveyAnswer)
        {
            if (newsurveyAnswer == null)
            {
                return BadRequest("No survey answer data was passed");
            }
            var existingData = await _context.SurveyCompletion
                .FirstOrDefaultAsync(s => s.UserId == newsurveyAnswer.UserId
                && s.SurveyId == newsurveyAnswer.SurveyId);
            if (existingData == null)
            {
                return NotFound("Survey not found.");
            }
            existingData.SurveyCompletionTypeId = 2; // 2 is for completed surveys
            existingData.SurveyCompletionDate = DateTime.Now;
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
        [HttpPost("DeleteSavedSurvey")]
        public async Task<IActionResult> DeleteSavedSurvey([FromBody] SurveySaveAnswerDto newsurveyAnswer)
        {
            if (newsurveyAnswer == null)
            {
                return BadRequest("No survey answer data was passed");
            }

            var survey = await _context.Surveys
                .FirstOrDefaultAsync(s => s.SurveyId == newsurveyAnswer.SurveyId);

            if (survey != null && survey.EndDate <= DateTime.Now)
            {
                return BadRequest("Cannot delete a survey that has already ended.");
            }

            var existingData = await _context.SurveyCompletion
                .Include(s => s.SurveyAnswers)
                .FirstOrDefaultAsync(s => s.UserId == newsurveyAnswer.UserId
                && s.SurveyId == newsurveyAnswer.SurveyId
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
