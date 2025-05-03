using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Survey.Domain.Entities
{
    public class SurveyAnswer
    {
        [Key]
        public int SurveyAnswerId { get; set; }
        [ForeignKey("Questionnaire")]
        public int QuestionnaireId { get; set; }
        public virtual Questionnaire? Questionnaire { get; set; }
        public string Answer { get; set; } = string.Empty;
        [JsonIgnore]
        [ForeignKey("SurveyCompletion")]
        public int SurveyCompletionId { get; set; }
        public virtual SurveyCompletion? SurveyCompletion { get; set; }

    }
}
