using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Survey.Application
{
    public class ExportSurvey
    {
        public string SurveyTitle { get; set; } = string.Empty;
        public string SurveyDescription { get; set; } = string.Empty;
        public DateTime StartDate { get; set; } = DateTime.Now;
        public DateTime EndDate { get; set; } = DateTime.Now;
        public string SurveyType { get; set; } = string.Empty;
        public List<ExportQuestionnaire> Questionnaires { get; set; } = [];

    }

    public class ExportQuestionnaire
    {
        public string QuestionnaireTitle { get; set; } = string.Empty;
        public string InputType { get; set; } = string.Empty;
        public string Range { get; set; } = string.Empty;
        public List<ExportMultipleChoice> Options { get; set; } = [];
    }

    public class ExportMultipleChoice
    {
        public string Option { get; set; } = string.Empty;
    }


}
