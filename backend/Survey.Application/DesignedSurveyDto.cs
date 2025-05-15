namespace Survey.Application
{
    public class DesignedSurveyDto
    {
        public int? SurveyId { get; set; }
        public string SurveyTitle { get; set; } = string.Empty;
        public string SurveyDescription { get; set; }  = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int SurveyTypeId { get; set; }
        public string? PrivateKey { get; set; }
        public int UserId { get; set; }
        
        public List<QuestionnaireDto>? Questionnaires { get; set; }

        public int ResponseCount { get; set; }
        public bool Published { get; set; } = false;

    }

    public class QuestionnaireDto
    {
        public int QuestionnaireId { get; set; }

        public int QuestionnairePos { get; set; }
        public string QuestionnaireTitle { get; set; } = string.Empty;
        public string InputType { get; set; } = string.Empty;
        public string Range { get; set; } = string.Empty;
        public int? SurveyId { get; set; }

        public List<MultipleChoiceDto> MultipleChoices { get; set; } = new();
    }
    
    public class MultipleChoiceDto
    {
        public int MultipleChoiceId { get; set; }
        public string MultipleChoiceName { get; set; } = string.Empty;
    }



}
