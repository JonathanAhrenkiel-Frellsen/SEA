using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Survey.Domain.Entities;

namespace Survey.Application
{
    public class SurveyAnswerDto
    {
        public int SurveyAnswerId { get; set; }
        public int SurveyId { get; set; }
        public int QuestionnaireId { get; set; }
        public string Answer { get; set; }
        public int SurveyCompletionId { get; set; }
    }
}
