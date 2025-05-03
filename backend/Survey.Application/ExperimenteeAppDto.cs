namespace Survey.Application
{
    public class ExperimenteeAppDto
    {
        public int? SurveyId { get; set; }
        public string? SurveyTitle { get; set; }
        public string? SurveyDescription { get; set; }
        public int? UserId { get; set; }
        public virtual ICollection<SurveyStoredAnwsersDto>? SurveyStoredAnwsers { get; set; }

    }

    public class SurveyStoredAnwsersDto
    {
        public int? QuestionnaireId { get; set; }
        public string? QuestionnaireTitle { get; set; }
        public string? InputType { get; set; }
        public string? Range { get; set; }
        public string? SurveyAnswer { get; set; }
        public virtual ICollection<MultipleChoicesDto>? MultipleChoices { get; set; }
    }

    public class MultipleChoicesDto
    {
        public int? MultipleChoiceId { get; set; }
        public string? MultipleChoiceName { get; set; }
    }

    public class SurveySaveAnswerDto
    {
        public int? QuestionnaireId { get; set; }
        public string? QuestionnaireTitle { get; set; }
        public string? InputType { get; set; }
        public string? Range { get; set; }
        public string? SurveyAnswer { get; set; }
        public int? SurveyId { get; set; }
        public int? UserId { get; set; }
    }

}

