using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Survey.Application
{
    public class ExportSurveyDto
    {
        public string SurveyTitle { get; set; }
        public string SurveyDescription { get; set; }

        public List<ExportQuestionnaire> Questionnaires { get; set; }

    }

    public class ExportQuestionnaire
    {
        public string QuestionnaireTitle { get; set; }
        public string InputType { get; set; }
        public string Range { get; set; }

    }
}
