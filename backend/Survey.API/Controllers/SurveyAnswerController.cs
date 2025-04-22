using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Survey.Domain.Entities;
using Survey.Infrastructure.Data;

namespace Survey.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SurveyAnswerController : ControllerBase
    {
        private readonly SurveyDbContext _context;

        public SurveyAnswerController(SurveyDbContext context)
        {
            _context = context;
        }

        // GET: api/SurveyAnswer
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SurveyAnswer>>> GetSurveyAnswers()
        {
            return await _context.SurveyAnswer.ToListAsync();
        }

        // GET: api/SurveyAnswer/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SurveyAnswer>> GetSurveyAnswer(int id)
        {
            var surveyAnswer = await _context.SurveyAnswer.FirstOrDefaultAsync(sa => sa.SurveyAnswerId == id);

            if (surveyAnswer == null)
            {
                return NotFound();
            }

            return surveyAnswer;
        }

        // POST: api/SurveyAnswer
        [HttpPost]
        public async Task<ActionResult<SurveyAnswer>> PostSurveyAnswer(SurveyAnswer surveyAnswer)
        {
            _context.SurveyAnswer.Add(surveyAnswer);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSurveyAnswer), new { id = surveyAnswer.SurveyAnswerId }, surveyAnswer);
        }

        // PUT: api/SurveyAnswer/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSurveyAnswer(int id, SurveyAnswer surveyAnswer)
        {
            if (id != surveyAnswer.SurveyAnswerId)
            {
                return BadRequest();
            }

            _context.Entry(surveyAnswer).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SurveyAnswerExists(id))
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

        // DELETE: api/SurveyAnswer/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSurveyAnswer(int id)
        {
            var surveyAnswer = await _context.SurveyAnswer.FindAsync(id);
            if (surveyAnswer == null)
            {
                return NotFound();
            }

            _context.SurveyAnswer.Remove(surveyAnswer);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SurveyAnswerExists(int id)
        {
            return _context.SurveyAnswer.Any(e => e.SurveyAnswerId == id);
        }
    }
}
