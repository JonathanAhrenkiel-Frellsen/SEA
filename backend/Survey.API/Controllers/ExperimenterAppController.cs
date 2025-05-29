using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Xml.Serialization;
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
    public class ExperimenterAppController : ControllerBase
    {
        private readonly SurveyDbContext _context;
        private readonly ILogger<ExperimenterAppController> _logger;

        public ExperimenterAppController(
            ILogger<ExperimenterAppController> logger,
            SurveyDbContext context)
        {
            _logger = logger;
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
        [HttpPost("surveys")]
        public async Task<ActionResult<DesignedSurvey>> SaveSurvey([FromBody] DesignedSurveyDto surveyDto)
        {
            if (surveyDto == null || surveyDto.Questionnaires == null)
            {
                return BadRequest("Survey data or questionnaires cannot be null");
            }

            if (surveyDto.StartDate > surveyDto.EndDate)
            {
                return BadRequest("Start date cannot be greater than end date");
            }

            if (surveyDto.SurveyId > 0)
            {
                var existingSurvey = await _context.Surveys
                    .Include(s => s.Questionnaires)
                        .ThenInclude(q => q.MultipleChoices)
                    .FirstOrDefaultAsync(s => s.SurveyId == surveyDto.SurveyId);

                if (existingSurvey == null)
                {
                    return NotFound($"Survey with ID {surveyDto.SurveyId} not found");
                }

                //Added a statement to check whether the survey is published
                if (existingSurvey.Published)
                {
                    return BadRequest($"Survey with ID {surveyDto.SurveyId} is already published and cannot be edited.");
                }

                existingSurvey.SurveyTitle = surveyDto.SurveyTitle;
                existingSurvey.SurveyDescription = surveyDto.SurveyDescription;
                existingSurvey.StartDate = surveyDto.StartDate;
                existingSurvey.EndDate = surveyDto.EndDate;
                existingSurvey.SurveyTypeId = surveyDto.SurveyTypeId;
                existingSurvey.UserId = surveyDto.UserId;
                existingSurvey.PrivateKey = surveyDto.PrivateKey;
                existingSurvey.Published = surveyDto.Published;

                foreach (var qDto in surveyDto.Questionnaires)
                {
                    var existingQuestionnaire = existingSurvey.Questionnaires
                        .FirstOrDefault(q => q.QuestionnaireId == qDto.QuestionnaireId);

                    if (existingQuestionnaire != null)
                    {
                        existingQuestionnaire.QuestionnaireTitle = qDto.QuestionnaireTitle;
                        existingQuestionnaire.InputType = qDto.InputType;
                        existingQuestionnaire.Range = qDto.Range;
                        existingQuestionnaire.QuestionnairePos = qDto.QuestionnairePos;

                        foreach (var mcDto in qDto.MultipleChoices ?? new List<MultipleChoiceDto>())
                        {
                            var existingMc = existingQuestionnaire.MultipleChoices?
                                .FirstOrDefault(mc => mc.MultipleChoiceId == mcDto.MultipleChoiceId);

                            if (existingMc != null)
                            {
                                existingMc.MultipleChoiceName = mcDto.MultipleChoiceName;
                            }
                            else
                            {
                                existingQuestionnaire.MultipleChoices ??= new List<MultipleChoice>();
                                existingQuestionnaire.MultipleChoices.Add(new MultipleChoice
                                {
                                    MultipleChoiceName = mcDto.MultipleChoiceName
                                });
                            }
                        }
                    }
                    else
                    {
                        existingSurvey.Questionnaires.Add(new Questionnaire
                        {
                            QuestionnaireTitle = qDto.QuestionnaireTitle,
                            InputType = qDto.InputType,
                            Range = qDto.Range,
                            QuestionnairePos = qDto.QuestionnairePos,
                            MultipleChoices = qDto.MultipleChoices?
                                .Select(mc => new MultipleChoice
                                {
                                    MultipleChoiceName = mc.MultipleChoiceName
                                }).ToList()
                        });
                    }
                }

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    return BadRequest("Error occurred while saving the survey. Please try again.");
                }

                return Ok(existingSurvey);
            }
            else
            {
                var newSurvey = new DesignedSurvey
                {
                    SurveyTitle = surveyDto.SurveyTitle,
                    SurveyDescription = surveyDto.SurveyDescription,
                    StartDate = surveyDto.StartDate,
                    EndDate = surveyDto.EndDate,
                    SurveyTypeId = surveyDto.SurveyTypeId,
                    UserId = surveyDto.UserId,
                    PrivateKey = surveyDto.PrivateKey,
                    Questionnaires = surveyDto.Questionnaires.Select(q => new Questionnaire
                    {
                        QuestionnaireTitle = q.QuestionnaireTitle,
                        InputType = q.InputType,
                        Range = q.Range,
                        QuestionnairePos = q.QuestionnairePos,
                        MultipleChoices = q.MultipleChoices?.Select(mc => new MultipleChoice
                        {
                            MultipleChoiceName = mc.MultipleChoiceName
                        }).ToList()
                    }).ToList()
                };

                _context.Surveys.Add(newSurvey);

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    return BadRequest("Error occurred while saving the survey. Please try again.");
                }

                return CreatedAtAction(nameof(SaveSurvey), new { id = newSurvey.SurveyId }, newSurvey);
            }
        }


        // GET: api/ExperimenterApp/surveys/{id}
        [HttpGet("surveys/{id}")]
        public async Task<IActionResult> GetSurvey(int id)
        {
            var survey = await _context.Surveys
                .Include(s => s.Questionnaires)
                .ThenInclude(q => q.MultipleChoices)
                .FirstOrDefaultAsync(s => s.SurveyId == id);
            if (survey == null) return NotFound();



            var dto = new DesignedSurveyDto
            {
                SurveyId = survey.SurveyId,
                SurveyTitle = survey.SurveyTitle,
                SurveyDescription = survey.SurveyDescription,
                StartDate = survey.StartDate,
                EndDate = survey.EndDate,
                SurveyTypeId = survey.SurveyTypeId,
                PrivateKey = survey.PrivateKey,
                Published = survey.Published,
                IsPaused = survey.IsPaused,  // NEW
                Questionnaires = survey.Questionnaires.Select(q => new QuestionnaireDto
                {
                    QuestionnaireId = q.QuestionnaireId,
                    QuestionnairePos = q.QuestionnairePos,
                    QuestionnaireTitle = q.QuestionnaireTitle,
                    InputType = q.InputType,
                    Range = q.Range,
                    SurveyId = q.SurveyId,
                    MultipleChoices = q.MultipleChoices.Select(mc => new MultipleChoiceDto
                    {
                        MultipleChoiceId = mc.MultipleChoiceId,
                        MultipleChoiceName = mc.MultipleChoiceName
                    }).ToList()
                }).ToList()
            };

            return Ok(dto);
        }

        // POST: api/ExperimenterApp/surveys/{id}/pause
        [HttpPost("surveys/{id}/pause")]
        public async Task<IActionResult> PauseSurvey(int id)
        {
            var survey = await _context.Surveys.FindAsync(id);
            if (survey == null) return NotFound();
            try
            {
                survey.Pause();
                await _context.SaveChangesAsync();
                return Ok(new { message = "Survey paused" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST: api/ExperimenterApp/surveys/{id}/resume
        [HttpPost("surveys/{id}/resume")]
        public async Task<IActionResult> ResumeSurvey(int id)
        {
            var survey = await _context.Surveys.FindAsync(id);
            if (survey == null) return NotFound();
            try
            {
                survey.Resume();
                await _context.SaveChangesAsync();
                return Ok(new { message = "Survey resumed" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }


        // POST: api/ExperimenterApp/PublishSurvey

        [HttpPost("surveys/{id}/publish")]
        public async Task<IActionResult> PublishSurvey(int id)
        {
            var survey = await _context.Surveys.FirstOrDefaultAsync(s => s.SurveyId == id);

            if (survey == null)
                return NotFound("Survey not found");

            if (survey.Published)
                return BadRequest("Survey is already published.");

            survey.Published = true;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Survey published successfully", shareUrl = $"[YOUR_BASE_URL]/surveys/{id}" });
        }


        // POST: api/ExperimenterApp/LoadSurvey
        [Authorize]
        [Pincode] // needs the surveyId form the HttpGET!
        [HttpGet("LoadSurvey/{surveyId}")]
        public async Task<ActionResult<DesignedSurvey>> LoadSurvey(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid survey ID");
            }

            var existingSurvey = await _context.Surveys
                .Include(s => s.Questionnaires.OrderBy(q => q.QuestionnairePos))
                .ThenInclude(q => q.MultipleChoices)
                .Include(s => s.SurveyType)
                .FirstOrDefaultAsync(s => s.SurveyId == id);

            if (existingSurvey == null)
            {
                return NotFound($"Survey with ID {id} not found");
            }

            return Ok(existingSurvey);
        }

        // GET: api/ExperimenterApp/GetPublicSurveys
        [Authorize]
        [HttpGet("surveys")]
        public async Task<ActionResult<IEnumerable<DesignedSurveyDto>>> GetPublicSurveys()
        {
            var userIdClaim = User.FindFirst("UserId");
            var isSuperUser = User.FindFirst("UserType")!.Value == "1";

            if (userIdClaim == null)
            {
                return Unauthorized("User ID claim not found in token.");
            }

            if (!int.TryParse(userIdClaim.Value, out int userId))
            {
                return BadRequest("Invalid user ID in token.");
            }

            var query = _context.Surveys
                .Where(s => s.EndDate <= DateTime.UtcNow);

            if (!isSuperUser)
            {
                query = query.Where(s => s.UserId == userId);
            }

            var surveys = await query.ToListAsync();

            //Added responsecount-dto
            var surveyIds = surveys.Select(s => s.SurveyId).ToList();

            var responseCounts = await _context.SurveyCompletion
                .Where(sc => sc.SurveyCompletionTypeId == 2 && surveyIds.Contains(sc.SurveyId))
                .GroupBy(sc => sc.SurveyId)
                .Select(g => new { SurveyId = g.Key, Count = g.Count() })
                .ToListAsync();

            var responseCountDict = responseCounts.ToDictionary(rc => rc.SurveyId, rc => rc.Count);

            var dto = surveys.Select(s => new DesignedSurveyDto
            {
                SurveyId = s.SurveyId,
                SurveyTitle = s.SurveyTitle,
                SurveyDescription = s.SurveyDescription,
                StartDate = s.StartDate,
                EndDate = s.EndDate,
                SurveyTypeId = s.SurveyTypeId,
                UserId = s.UserId,
                ResponseCount = responseCountDict.ContainsKey(s.SurveyId) ? responseCountDict[s.SurveyId] : 0,
                Published = s.Published,
                IsPaused = s.IsPaused
            }).ToList();

            return Ok(dto);
        }

        // POST: api/ExperimenterApp/DeleteSurvey
        [Authorize]
        [HttpDelete("surveys/{id}")]
        public async Task<ActionResult> DeleteSurvey(int id)
        {
            var userId = User.FindFirst("UserId")!.Value;
            var isSuperUser = User.FindFirst("UserType")!.Value == "1";

            if (id <= 0)
            {
                return BadRequest("Invalid survey ID");
            }

            var existingSurvey = await _context.Surveys
                .Include(s => s.Questionnaires)
                .ThenInclude(q => q.MultipleChoices)
                .FirstOrDefaultAsync(s => s.SurveyId == id);

            if (existingSurvey == null)
            {
                return NotFound($"Survey with ID {id} not found");
            }

            if (!isSuperUser && userId != existingSurvey.UserId.ToString())
            {
                return Forbid();
            }

            _context.Surveys.Remove(existingSurvey);
            await _context.SaveChangesAsync();

            return Ok("Survey has been deleted.");
        }

        // POST: api/ExperimenterApp/ExportSurvey
        [Authorize]
        [HttpGet("ExportSurvey/:id")]
        public async Task<ActionResult<ExportSurvey>> ExportSurvey(int id)
        {
            var userId = User.FindFirst("UserId")!.Value;
            var isSuperUser = User.FindFirst("UserType")!.Value == "1";

            if (id <= 0)
            {
                return BadRequest("Invalid survey data");
            }
            var existingSurvey = await _context.Surveys
                .Include(s => s.SurveyType)
                .Include(s => s.Questionnaires)
                    .ThenInclude(q => q.MultipleChoices)
                .FirstOrDefaultAsync(s => s.SurveyId == id);
            if (existingSurvey == null)
            {
                return NotFound($"Survey with ID {id} not found");
            }

            if (!isSuperUser && userId != existingSurvey.UserId.ToString())
            {
                return Forbid();
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
        public async Task<IActionResult> ImportSurvey([FromForm] ImportSurveyDto model)
        {
            if (model.File == null || model.File.Length == 0)
            {
                return BadRequest("No file uploaded");
            }

            using var stream = new MemoryStream();
            await model.File.CopyToAsync(stream);
            string xmlContent = Encoding.UTF8.GetString(stream.ToArray());

            var survey = XmlHelper.DeserializeFromXml<ExportSurvey>(xmlContent);
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
                return NotFound($"This survey is yet to be completed.");
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
                return NotFound($"This survey is yet to be filled up.");
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








        [Authorize]
        [HttpGet("surveys/{surveyId}/export")]
        public async Task<IActionResult> ExportSurveyResponsesToCsv(int surveyId, [FromQuery] string type = "completed")
        {
            var isCompleted = type.ToLower() == "completed";
            var completionTypeId = isCompleted ? 2 : 1;

            var completions = await _context.SurveyCompletion
                .Include(sc => sc.User)
                .Include(sc => sc.SurveyAnswers)
                    .ThenInclude(a => a.Questionnaire)
                .Where(sc => sc.SurveyId == surveyId && sc.SurveyCompletionTypeId == completionTypeId)
                .ToListAsync();

            if (completions.Count == 0)
                return NotFound("No survey completions found.");

            var questionTitles = completions
                .SelectMany(c => c.SurveyAnswers)
                .Select(a => a.Questionnaire?.QuestionnaireTitle ?? $"Q{a.QuestionnaireId}")
                .Distinct()
                .ToList();

            var csv = new StringBuilder();
            csv.AppendLine("Name,Email,Date," + string.Join(",", questionTitles));

            foreach (var c in completions)
            {
                var row = new List<string>
                {
                    c.User?.UserName ?? "Unknown",
                    c.User?.UserEmail ?? "Unknown",
                    c.SurveyCompletionDate.ToString("yyyy-MM-dd HH:mm:ss")
                };

                foreach (var title in questionTitles)
                {
                    var answer = c.SurveyAnswers
                        .FirstOrDefault(a => a.Questionnaire?.QuestionnaireTitle == title)
                        ?.Answer ?? "";
                    row.Add(answer.Replace(",", " "));
                }

                csv.AppendLine(string.Join(",", row));
            }

            var fileName = $"survey_{surveyId}_{type}_results.csv";
            return File(Encoding.UTF8.GetBytes(csv.ToString()), "text/csv", fileName);
        }
        [Authorize]
        [HttpGet("surveys/{id}/structure-export")]
        public async Task<IActionResult> ExportSurveyStructure(int id)
        {
            var survey = await _context.Surveys
                .Include(s => s.Questionnaires)
                    .ThenInclude(q => q.MultipleChoices)
                .FirstOrDefaultAsync(s => s.SurveyId == id);

            if (survey == null)
                return NotFound("Survey not found");

            var isPublic = string.IsNullOrEmpty(survey.PrivateKey);

            var csv = new StringBuilder();
            csv.AppendLine("Survey Title,Accessibility,Question,Answer Type,Answer Options");

            foreach (var q in survey.Questionnaires.OrderBy(q => q.QuestionnairePos))
            {
                var options = q.InputType == "checkbox"
                    ? string.Join(" | ", q.MultipleChoices?.Select(mc => mc.MultipleChoiceName) ?? [])
                    : "";

                csv.AppendLine(
                    $"\"{survey.SurveyTitle}\"," +
                    $"{(isPublic ? "Public" : "Private")}," +
                    $"\"{q.QuestionnaireTitle}\"," +
                    $"{q.InputType}," +
                    $"\"{options}\""
                );
            }

            var fileName = $"survey_{id}_structure.csv";
            return File(Encoding.UTF8.GetBytes(csv.ToString()), "text/csv", fileName);
        }



    }
}
