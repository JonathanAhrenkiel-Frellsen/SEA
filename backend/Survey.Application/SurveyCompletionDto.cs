using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Survey.Application
{
    public class SurveyCompletionDto
    {
        public int SurveyCompletionId { get; set; }
        public DateTime SurveyCompletionDate { get; set; }
        public int SurveyId { get; set; }
        public int UserId;
    }
}
