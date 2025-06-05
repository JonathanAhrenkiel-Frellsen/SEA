using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using SurveyDbContext = Survey.Infrastructure.Data.SurveyDbContext;

namespace Survey.API.Attributes;

public class PincodeAttribute : Attribute, IAsyncAuthorizationFilter
{
    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        // var surveyIdStr = context.RouteData.Values["id"]?.ToString();
        var key = context.RouteData.Values.ContainsKey("id") ? "id" : "surveyId";
        var surveyIdStr = context.RouteData.Values[key]?.ToString();
        if (!int.TryParse(surveyIdStr, out var surveyId) || surveyId <= 0)
        {
            context.Result = new BadRequestObjectResult("Invalid survey ID.");
            return;
        }

        if (context.HttpContext.RequestServices.GetService(typeof(SurveyDbContext)) is not SurveyDbContext dbContext)
        {
            context.Result = new StatusCodeResult(500);
            return;
        }

        var survey = await dbContext.Surveys
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.SurveyId == surveyId);

        if (survey == null)
        {
            context.Result = new NotFoundObjectResult("Survey not found.");
            return;
        }

        var userIdClaim = context.HttpContext.User.FindFirst("UserId");
        if (int.TryParse(userIdClaim?.Value, out var userId))
        {
            if (survey.UserId == userId)
            {
                // Creator is accessing; skip PIN check
                return;
            }
        }

        // Check if PIN is needed
        if (string.IsNullOrEmpty(survey.PrivateKey))
            return; // No PIN required

        if (!context.HttpContext.Request.Headers.TryGetValue("X-Survey-Pin", out var pin) ||
            survey.PrivateKey != pin)
        {
            context.Result = new ContentResult
            {
                StatusCode = 403,
                Content = "Invalid or missing PIN."
            };
        }
    }
}