using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Survey.Domain.Entities;
using Survey.Infrastructure.Data;

namespace Survey.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SurveyCompletionController : ControllerBase
    {
        private readonly SurveyDbContext _context;

        public SurveyCompletionController(SurveyDbContext context)
        {
            _context = context;
        }

        // GET: api/SurveyCompletion
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SurveyCompletion>>> GetSurveyCompletions()
        {
            return await _context.SurveyCompletion
                .Include(sc => sc.SurveyAnswers) // Include related SurveyAnswers
                .ToListAsync();
        }

        // GET: api/SurveyCompletion/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SurveyCompletion>> GetSurveyCompletion(int id)
        {
            var surveyCompletion = await _context.SurveyCompletion
                .Include(sc => sc.SurveyAnswers) // Include related SurveyAnswers
                .FirstOrDefaultAsync(sc => sc.SurveyCompletionId == id);

            if (surveyCompletion == null)
            {
                return NotFound();
            }

            return surveyCompletion;
        }

        // POST: api/SurveyCompletion
        [HttpPost]
        public async Task<ActionResult<SurveyCompletion>> PostSurveyCompletion(SurveyCompletion surveyCompletion)
        {
            _context.SurveyCompletion.Add(surveyCompletion);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSurveyCompletion), new { id = surveyCompletion.SurveyCompletionId }, surveyCompletion);
        }

        // PUT: api/SurveyCompletion/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSurveyCompletion(int id, SurveyCompletion surveyCompletion)
        {
            if (id != surveyCompletion.SurveyCompletionId)
            {
                return BadRequest();
            }

            _context.Entry(surveyCompletion).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SurveyCompletionExists(id))
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

        // DELETE: api/SurveyCompletion/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSurveyCompletion(int id)
        {
            var surveyCompletion = await _context.SurveyCompletion.FindAsync(id);
            if (surveyCompletion == null)
            {
                return NotFound();
            }

            _context.SurveyCompletion.Remove(surveyCompletion);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SurveyCompletionExists(int id)
        {
            return _context.SurveyCompletion.Any(e => e.SurveyCompletionId == id);
        }
    }
}
