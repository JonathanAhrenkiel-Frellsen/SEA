using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Survey.Domain.Entities;
using Survey.Infrastructure.Data;

namespace Survey.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SurveyController : ControllerBase
    {
        private readonly SurveyDbContext _context;

        public SurveyController(SurveyDbContext context)
        {
            _context = context;
        }

        // GET: api/Survey
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DesignedSurvey>>> GetSurveys()
        {
            return await _context.Surveys.Include(s => s.Questionnaires).ToListAsync();
        }

        // GET: api/Survey/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DesignedSurvey>> GetSurvey(int id)
        {
            var survey = await _context.Surveys.Include(s => s.Questionnaires).FirstOrDefaultAsync(s => s.SurveyId == id);

            if (survey == null)
            {
                return NotFound();
            }

            return survey;
        }

        // POST: api/Survey
        [HttpPost]
        public async Task<ActionResult<DesignedSurvey>> PostSurvey(DesignedSurvey survey)
        {

            _context.Surveys.Add(survey);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSurvey), new { id = survey.SurveyId }, survey);
        }

        // PUT: api/Survey/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSurvey(int id, DesignedSurvey survey)
        {
            if (id != survey.SurveyId)
            {
                return BadRequest();
            }

            _context.Entry(survey).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SurveyExists(id))
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

        // DELETE: api/Survey/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSurvey(int id)
        {
            var survey = await _context.Surveys.FindAsync(id);
            if (survey == null)
            {
                return NotFound();
            }

            _context.Surveys.Remove(survey);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SurveyExists(int id)
        {
            return _context.Surveys.Any(e => e.SurveyId == id);
        }
    }
}
