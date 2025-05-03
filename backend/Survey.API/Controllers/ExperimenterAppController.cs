using System.Text;
using System.Xml.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Survey.Application;
using Survey.Domain.Entities;
using SurveyDbContext = Survey.Infrastructure.Data.SurveyDbContext;

namespace Survey.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExperimenterAppController : ControllerBase
    {
        private readonly SurveyDbContext _context;

        public ExperimenterAppController(SurveyDbContext context)
        {
            _context = context;
        }

        public static class XmlHelper
        {
            public static string SerializeToXml<T>(T data)
            {
                var serializer = new XmlSerializer(typeof(T));

                using var stringWriter = new StringWriter();
                serializer.Serialize(stringWriter, data);
                return stringWriter.ToString();
            }
            public static T DeserializeFromXml<T>(string xmlData)
            {
                var serializer = new XmlSerializer(typeof(T));

                using var stringReader = new StringReader(xmlData);
                var result = serializer.Deserialize(stringReader);
                if (result != null)
                {
                    return (T)result;
                }
                throw new InvalidOperationException("Deserialization resulted in a null object.");
            }
        }

        // POST: api/ExperimenterApp/SaveSurvey
        [HttpPost("SaveSurvey")]
        public async Task<ActionResult<DesignedSurvey>> SaveSurvey(DesignedSurvey survey)
        {
            if (survey == null || survey.Questionnaires == null)
            {
                return BadRequest("Survey data or questionnaires cannot be null");
            }

            if (survey.StartDate > survey.EndDate)
            {
                return BadRequest("Start date cannot be greater than end date");
            }

            if (survey.SurveyId <= 0)
            {
                // Check if the survey already exists
                var existingSurvey = await _context.Surveys
                    .Include(s => s.Questionnaires)
                        .ThenInclude(q => q.MultipleChoices)
                    .FirstOrDefaultAsync(s => s.SurveyId == survey.SurveyId);

                if (existingSurvey == null)
                {
                    return NotFound($"Survey with ID {survey.SurveyId} not found");
                }

                // Update existing survey
                existingSurvey.SurveyId = survey.SurveyId;
                existingSurvey.SurveyTitle = survey.SurveyTitle;
                existingSurvey.SurveyDescription = survey.SurveyDescription;
                existingSurvey.StartDate = survey.StartDate;
                existingSurvey.EndDate = survey.EndDate;
                existingSurvey.SurveyTypeId = survey.SurveyTypeId;
                existingSurvey.UserId = survey.UserId;
                existingSurvey.Questionnaires.Clear(); // Clear existing questionnaires
                foreach (var questionnaire in survey.Questionnaires)
                {
                    questionnaire.SurveyId = existingSurvey.SurveyId; // Set the foreign key
                    questionnaire.QuestionnaireId = questionnaire.QuestionnaireId <= 0 ? 0 : questionnaire.QuestionnaireId; // Set to 0 for new questionnaire 
                    _context.Entry(questionnaire).State = questionnaire.QuestionnaireId <= 0 ? EntityState.Added : EntityState.Modified;

                    if (questionnaire.MultipleChoices == null)
                    {
                        questionnaire.MultipleChoices = []; // Initialize if null
                    }
                    else
                    {
                        questionnaire.MultipleChoices.Clear(); // Clear existing multiple choices
                    }

                    foreach (var multipleChoice in questionnaire.MultipleChoices)
                    {
                        multipleChoice.QuestionnaireId = questionnaire.QuestionnaireId; // Set the foreign key
                        multipleChoice.MultipleChoiceId = multipleChoice.MultipleChoiceId <= 0 ? 0 : multipleChoice.MultipleChoiceId; // Set to 0 for new multiple choice
                        _context.Entry(multipleChoice).State = multipleChoice.MultipleChoiceId <= 0 ? EntityState.Added : EntityState.Modified;
                    }
                    existingSurvey.Questionnaires.Add(questionnaire); // Add the questionnaire to the existing survey
                }

                if (existingSurvey.SurveyTypeId == 1 && survey.SurveyTypeId == 2)
                {
                    existingSurvey.PrivateKey = Guid.NewGuid().ToString();
                }
                else if (existingSurvey.SurveyTypeId == 2 && survey.SurveyTypeId == 1)
                {
                    existingSurvey.PrivateKey = null; // Set to null if not needed
                }
                else if (existingSurvey.SurveyTypeId == 2 && survey.SurveyTypeId == 2)
                {
                    existingSurvey.PrivateKey = existingSurvey.PrivateKey; // Keep the same private key
                }
                else if (existingSurvey.SurveyTypeId == 1 && survey.SurveyTypeId == 1)
                {
                    existingSurvey.PrivateKey = null; // Set to null if not needed
                }

                // Update the survey in the database
                _context.Entry(existingSurvey).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    return BadRequest("Error Occured while saving the survey. Please try again.");
                }
                return Ok(existingSurvey);
            }
            else
            {
                survey.SurveyId = 0; // Set to 0 for new survey

                foreach (var questionnaire in survey.Questionnaires)
                {
                    questionnaire.SurveyId = survey.SurveyId; // Set the foreign key
                    questionnaire.QuestionnaireId = 0; // Set to 0 for new questionnaire
                    questionnaire.MultipleChoices ??= []; // Initialize if null
                    foreach (var multipleChoice in questionnaire.MultipleChoices)
                    {
                        multipleChoice.QuestionnaireId = questionnaire.QuestionnaireId; // Set the foreign key
                        multipleChoice.MultipleChoiceId = 0; // Set to 0 for new multiple choice
                        _context.Entry(multipleChoice).State = EntityState.Added;
                    }
                    _context.Entry(questionnaire).State = EntityState.Added;
                }

                if (survey.SurveyTypeId == 2)
                {
                    survey.PrivateKey = Guid.NewGuid().ToString();
                }
                else
                {
                    survey.PrivateKey = null; // Set to null if not needed
                }

                // Add the new survey to the database
                _context.Entry(survey).State = EntityState.Added;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    return BadRequest("Error Occured while saving the survey. Please try again.");
                }

                // Return the created survey with its ID
                return CreatedAtAction(nameof(SaveSurvey), new { id = survey.SurveyId }, survey);
            }
        }

        // POST: api/ExperimenterApp/LoadSurvey
        [HttpPost("LoadSurvey")]
        public async Task<ActionResult<DesignedSurvey>> LoadSurvey([FromBody] DesignedSurveyDto survey)
        {
            if (survey == null || survey.SurveyId <= 0)
            {
                return BadRequest("Invalid survey data");
            }
            var existingSurvey = await _context.Surveys
                .Include(s => s.Questionnaires)
                    .ThenInclude(q => q.MultipleChoices)
                .Include(s => s.SurveyType)
                .FirstOrDefaultAsync(s => s.SurveyId == survey.SurveyId);
            if (existingSurvey == null)
            {
                return NotFound($"Survey with ID {survey.SurveyId} not found");
            }
            return Ok(existingSurvey);
        }

        // GET: api/ExperimenterApp/GetPublicSurveys
        [HttpGet("GetPublicSurveys")]
        public async Task<ActionResult<IEnumerable<DesignedSurvey>>> GetPublicSurveys()
        {
            var surveys = await _context.Surveys
                .Where(s => s.SurveyTypeId == 1 && s.EndDate <= DateTime.Now)
                .ToListAsync();
            if (surveys == null || surveys.Count == 0)
            {
                return NotFound("No public surveys found");
            }

            var dto = surveys.Select(s => new DesignedSurveyDto
            {
                SurveyId = s.SurveyId,
                SurveyTitle = s.SurveyTitle,
                SurveyDescription = s.SurveyDescription,
                StartDate = s.StartDate,
                EndDate = s.EndDate,
                SurveyTypeId = s.SurveyTypeId,
                UserId = s.UserId
            }).ToList();

            return Ok(dto);
        }

        // GET: api/ExperimenterApp/GetPrivateSurveys
        [HttpGet("GetPrivateSurveys")]
        public async Task<ActionResult<IEnumerable<DesignedSurvey>>> GetPrivateSurveys()
        {
            var surveys = await _context.Surveys
                .Where(s => s.SurveyTypeId == 2 && s.EndDate <= DateTime.Now)
                .ToListAsync();
            if (surveys == null || surveys.Count == 0)
            {
                return NotFound("No private surveys found");
            }

            var dto = surveys.Select(s => new DesignedSurveyDto
            {
                SurveyId = s.SurveyId,
                SurveyTitle = s.SurveyTitle,
                SurveyDescription = s.SurveyDescription,
                StartDate = s.StartDate,
                EndDate = s.EndDate,
                SurveyTypeId = s.SurveyTypeId,
                UserId = s.UserId
            }).ToList();

            return Ok(dto);
        }

        // POST: api/ExperimenterApp/DeleteSurvey
        [HttpPost("DeleteSurvey")]
        public async Task<ActionResult<DesignedSurvey>> DeleteSurvey(DesignedSurveyDto survey)
        {
            if (survey == null)
            {
                return BadRequest("Data cannot be null");
            }

            if (survey.SurveyId <= 0)
            {
                return BadRequest("Invalid survey ID");
            }

            var existingSurvey = await _context.Surveys
                .Include(s => s.Questionnaires)
                    .ThenInclude(q => q.MultipleChoices)
                .FirstOrDefaultAsync(s => s.SurveyId == survey.SurveyId);

            if (existingSurvey == null)
            {
                return NotFound($"Survey with ID {survey.SurveyId} not found");
            }

            _context.Surveys.Remove(existingSurvey);
            await _context.SaveChangesAsync();
            return Ok("Survey has been deleted.");
        }

        // POST: api/ExperimenterApp/ExportSurvey
        [HttpPost("ExportSurvey")]
        public async Task<ActionResult<ExportSurvey>> ExportSurvey(DesignedSurveyDto survey)
        {
            if (survey == null || survey.SurveyId <= 0)
            {
                return BadRequest("Invalid survey data");
            }
            var existingSurvey = await _context.Surveys
                .Include(s => s.SurveyType)
                .Include(s => s.Questionnaires)
                    .ThenInclude(q => q.MultipleChoices)
                .FirstOrDefaultAsync(s => s.SurveyId == survey.SurveyId);
            if (existingSurvey == null)
            {
                return NotFound($"Survey with ID {survey.SurveyId} not found");
            }

            var exportSurvey = new ExportSurvey
            {
                SurveyTitle = existingSurvey.SurveyTitle,
                SurveyDescription = existingSurvey.SurveyDescription,
                StartDate = existingSurvey.StartDate,
                EndDate = existingSurvey.EndDate,
                SurveyType = existingSurvey.SurveyType?.SurveyTypeName ?? "Unknown",
                Questionnaires = [.. existingSurvey.Questionnaires.Select(q => new ExportQuestionnaire
                {
                    QuestionnaireTitle = q.QuestionnaireTitle,
                    InputType = q.InputType,
                    Options = q.MultipleChoices?.Select(mc => new ExportMultipleChoice
                    {
                        Option = mc.MultipleChoiceName
                    }).ToList() ?? [],
                    Range = q.Range
                })]
            };

            var xmlData = XmlHelper.SerializeToXml(exportSurvey);
            var bytes = Encoding.UTF8.GetBytes(xmlData);

            return File(bytes, "application/xml", "SurveyData.xml");
        }

        // POST: api/ExperimenterApp/ImportSurvey
        [HttpPost("ImportSurvey")]
        public async Task<ActionResult<DesignedSurvey>> ImportSurvey([FromBody] string xmlData)
        {
            if (string.IsNullOrEmpty(xmlData))
            {
                return BadRequest("XML data cannot be null or empty");
            }
            var survey = XmlHelper.DeserializeFromXml<ExportSurvey>(xmlData);
            if (survey == null)
            {
                return BadRequest("Invalid XML data");
            }
            var designedSurvey = new DesignedSurvey
            {
                SurveyTitle = survey.SurveyTitle,
                SurveyDescription = survey.SurveyDescription,
                StartDate = survey.StartDate,
                EndDate = survey.EndDate,
                SurveyTypeId = survey.SurveyType == "Public" ? 1 : 2,
                Questionnaires = [.. survey.Questionnaires.Select(q => new Questionnaire
                {
                    QuestionnaireTitle = q.QuestionnaireTitle,
                    InputType = q.InputType,
                    Range = q.Range,
                    MultipleChoices = [.. q.Options.Select(o => new MultipleChoice
                    {
                        MultipleChoiceName = o.Option
                    })]
                })]
            };

            return Ok(designedSurvey);
        }

        // POST: api/ExperimenterApp/ExportSurveyResults
        [HttpPost("ExportCompletedSurveyResults")]
        public async Task<ActionResult<ExportSurvey>> ExportCompletedSurveyResults(DesignedSurveyDto survey)
        {
            if (survey == null || survey.SurveyId <= 0)
            {
                return BadRequest("Invalid survey data");
            }

            var existingSurveyResult = await _context.SurveyCompletion
                .Include(u => u.User)
                .Include(a => a.SurveyAnswers)
                    .ThenInclude(q => q.Questionnaire)
                .FirstOrDefaultAsync(s => s.SurveyId == survey.SurveyId && s.SurveyCompletionTypeId == 2);
            
            if (existingSurveyResult == null)
            {
                return NotFound($"Survey with ID {survey.SurveyId} not found");
            }
            
            var csvData = new StringBuilder();

            var header = "Expermintee Name, Expermintee Email, Completion Date";

            foreach (var answer in existingSurveyResult.SurveyAnswers)
            {
                header += $", {answer.Questionnaire?.QuestionnaireTitle ?? "Unknown"}";
            }

            csvData.AppendLine(header);
                

            foreach (var answer in existingSurveyResult.SurveyAnswers)
            {
                var row = $"{existingSurveyResult.User?.UserName ?? "Unknown"}, {existingSurveyResult.User?.UserEmail ?? "Unknown"}, {existingSurveyResult.SurveyCompletionDate}";
                row += $", {answer.Answer}";
                csvData.AppendLine(row);
            }

            

            var bytes = Encoding.UTF8.GetBytes(csvData.ToString());
            var fileName = $"{existingSurveyResult.Survey?.SurveyTitle ?? "Unknown"}_Completed_Results.csv";

            return File(bytes, "text/csv", fileName);
        }

        // POST: api/ExperimenterApp/ExportSurveyResults
        [HttpPost("ExportSavedSurveyResults")]
        public async Task<ActionResult<ExportSurvey>> ExportSavedSurveyResults(DesignedSurveyDto survey)
        {
            if (survey == null || survey.SurveyId <= 0)
            {
                return BadRequest("Invalid survey data");
            }

            var existingSurveyResult = await _context.SurveyCompletion
                .Include(u => u.User)
                .Include(a => a.SurveyAnswers)
                    .ThenInclude(q => q.Questionnaire)
                .Where(s => s.SurveyId == survey.SurveyId && s.SurveyCompletionTypeId == 1)
                .ToListAsync();

            if (existingSurveyResult == null)
            {
                return NotFound($"Survey with ID {survey.SurveyId} not found");
            }

            var csvData = new StringBuilder();

            var header = "Expermintee Name, Expermintee Email, Completion Date";

            foreach (var answer in existingSurveyResult[0].SurveyAnswers)
            {
                header += $", {answer.Questionnaire?.QuestionnaireTitle ?? "Unknown"}";
            }

            csvData.AppendLine(header);


            foreach (var user in existingSurveyResult)
            {
                var row = $"{user.User?.UserName ?? "Unknown"}, {user.User?.UserEmail ?? "Unknown"}, {user.SurveyCompletionDate}";

                foreach (var answer in user.SurveyAnswers)
                {
                    row += $", {answer.Answer}";
                }
                csvData.AppendLine(row);
            }



            var bytes = Encoding.UTF8.GetBytes(csvData.ToString());
            // Updated line to handle possible null reference
            var fileName = $"{existingSurveyResult[0]?.Survey?.SurveyTitle ?? "Unknown"}_Completed_Results.csv";

            return File(bytes, "text/csv", fileName);
        }
    }
}
