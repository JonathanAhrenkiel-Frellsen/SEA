using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Survey.Domain.Entities;
using Survey.Infrastructure.Data;

namespace Survey.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionnaireController : ControllerBase
    {
        private readonly SurveyDbContext _context;

        public QuestionnaireController(SurveyDbContext context)
        {
            _context = context;
        }

        // GET: api/Questionnaire
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Questionnaire>>> GetQuestionnaires()
        {
            return await _context.Questionnaires.ToListAsync();
        }

        // GET: api/Questionnaire/5
        [HttpGet("{id}")]
        //public async Task<ActionResult<Questionnaire>> GetQuestionnaire(int id)
        public async Task<ActionResult<IEnumerable<Questionnaire>>> GetQuestionnaire(int id)
        {
            //var questionnaire = await _context.Questionnaires.FindAsync(id);

            var questionnaire = await _context.Questionnaires.Where(q => q.SurveyId == id).ToListAsync();

            if (questionnaire == null)
            {
                return NotFound();
            }

            return questionnaire;
        }

        // POST: api/Questionnaire
        [HttpPost]
        public async Task<ActionResult<Questionnaire>> PostQuestionnaire(Questionnaire questionnaire)
        {
            _context.Questionnaires.Add(questionnaire);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetQuestionnaire), new { id = questionnaire.QuestionnaireId }, questionnaire);
        }

        // PUT: api/Questionnaire/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutQuestionnaire(int id, Questionnaire questionnaire)
        {
            if (id != questionnaire.QuestionnaireId)
            {
                return BadRequest();
            }

            _context.Entry(questionnaire).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!QuestionnaireExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Questionnaire/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQuestionnaire(int id)
        {
            var questionnaire = await _context.Questionnaires.FindAsync(id);
            if (questionnaire == null)
            {
                return NotFound();
            }

            _context.Questionnaires.Remove(questionnaire);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool QuestionnaireExists(int id)
        {
            return _context.Questionnaires.Any(e => e.QuestionnaireId == id);
        }
    }
}
