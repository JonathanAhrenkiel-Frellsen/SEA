namespace Survey.Application
{
    public class SurveyResponseOverTimeDto
    {
        public DateTime Date { get; set; }
        public int Count { get; set; }
    }
    
    public class CompletionHistogramBucketDto
    {
        public int AnsweredCount { get; set; }
        public int UserCount { get; set; }
    }

    public class SurveyCompletionRateDto
    {
        public int TotalQuestions { get; set; }
        public List<CompletionHistogramBucketDto> Histogram { get; set; } = new();
    }
}