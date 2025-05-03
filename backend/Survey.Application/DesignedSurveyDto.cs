namespace Survey.Application
{
    public class DesignedSurveyDto
    {
        public int? SurveyId { get; set; }
        public string? SurveyTitle { get; set; }
        public string? SurveyDescription { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? SurveyTypeId { get; set; }
        public string? PrivateKey { get; set; }
        public int? UserId { get; set; }
    }
}
