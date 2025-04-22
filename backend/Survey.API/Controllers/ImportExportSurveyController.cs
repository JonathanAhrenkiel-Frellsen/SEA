using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Survey.Application;
using Survey.Domain.Entities;
using Survey.Infrastructure.Data;
using System.Xml.Serialization;
using System.IO;
using Castle.Core.Resource;
using System.Text;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Survey.API.Controllers
{
    

    public static class XmlHelper
    {
        public static string SerializeToXml<T>(T data)
        {
            var serializer = new XmlSerializer(typeof(T));

            using (var stringWriter = new StringWriter())
            {
                serializer.Serialize(stringWriter, data);
                return stringWriter.ToString();
            }
        }
        public static T DeserializeFromXml<T>(string xmlData)
        {
            var serializer = new XmlSerializer(typeof(T));

            using (var stringReader = new StringReader(xmlData))
            {
                return (T)serializer.Deserialize(stringReader);
            }
        }
    }



    [Route("api/")]
    [ApiController]
    public class ImportExportSurveyController : ControllerBase
    {
        private readonly Infrastructure.Data.SurveyDbContext _context;

        public ImportExportSurveyController(Infrastructure.Data.SurveyDbContext context)
        {
            _context = context;
        }

        // GET api/<exportsurvey>/5
        [HttpGet("exportsurvey/{id}")]
        public async Task<ActionResult<DesignedSurvey>> ExportSurvey(int id)
        {
            var survey = await _context.Surveys.Include(s => s.Questionnaires).FirstOrDefaultAsync(s => s.SurveyId == id);

            if (survey == null)
            {
                return NotFound();
            }

            var dto = new ExportSurveyDto
            {
                SurveyTitle = survey.SurveyTitle,
                SurveyDescription = survey.SurveyDescription,
                Questionnaires = survey.Questionnaires.Select(q => new ExportQuestionnaire
                {
                    QuestionnaireTitle = q.QuestionnaireTitle,
                    InputType = q.InputType,
                    Range = q.Range
                }).ToList()
            };

            var xmlData = XmlHelper.SerializeToXml(dto);
            var bytes = Encoding.UTF8.GetBytes(xmlData);

            return File(bytes, "application/xml", "SurveyData.xml");
        }

        [HttpPost("importsurvey")]
        public IActionResult ImportFromXml(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Invalid file.");
            }

            try
            {
                using (var reader = new StreamReader(file.OpenReadStream()))
                {
                    var xmlData = reader.ReadToEnd();

                    var dto = XmlHelper.DeserializeFromXml<ExportSurveyDto>(xmlData);

                    var survey = new DesignedSurvey
                    {
                        SurveyTitle = dto.SurveyTitle,
                        SurveyDescription = dto.SurveyDescription,
                        Questionnaires = dto.Questionnaires.Select(q => new Questionnaire
                        {
                            QuestionnaireTitle = q.QuestionnaireTitle,
                            InputType = q.InputType,
                            Range = q.Range
                        }).ToList()
                    };
                    return Ok(survey);
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Error processing file: {ex.Message}");
            }

        }
    }
}
