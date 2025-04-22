using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Survey.Application
{
    internal class QuestionnaireDto
    {
        public int QuestionnaireId { get; set; }
        public string QuestionnaireTitle { get; set; }
        public string InputType { get; set; }
        public string Range { get; set; }
        public int SurveyId { get; set; }
    }
}
