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
    public class Questionnaire
    {
        [Key]
        public int QuestionnaireId { get; set; }
        public string QuestionnaireTitle { get; set; } = string.Empty;
        public string InputType { get; set; } = string.Empty;
        public string Range { get; set; } = string.Empty;
        [ForeignKey("DesignedSurvey")]
        public int? SurveyId { get; set; }
        [JsonIgnore]
        public virtual DesignedSurvey? Survey { get; set; }
        public virtual ICollection<MultipleChoice>? MultipleChoices { get; set; }

        public virtual ICollection<SurveyAnswer>? SurveyAnswers { get; set; }

        public Questionnaire()
        {
            MultipleChoices = [];
            SurveyAnswers = [];
        }

    }
}
