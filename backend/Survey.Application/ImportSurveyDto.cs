using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Survey.Application
{
    public class ImportSurveyDto
    {
        public string SurveyTitle { get; set; }
        public string SurveyDescription { get; set; }
        public List<ImportQuestionnaire> Questionnaires { get; set; }

    }

    public class ImportQuestionnaire
    {
        public string QuestionnaireTitle { get; set; }
        public string InputType { get; set; }
        public string Range { get; set; }

    }
}
