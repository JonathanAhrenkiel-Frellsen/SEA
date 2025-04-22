using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Survey.Domain.Entities
{
    public class MultipleChoice
    {
        public int MultipleChoiceId { get; set; }
        public string MultipleChoiceName { get; set; }
        [ForeignKey("Questionnaire")]
        [JsonIgnore]
        public int QuestionnaireId { get; set; }
        [JsonIgnore]
        public virtual Questionnaire Questionnaire { get; set; }

        public MultipleChoice()
        {
        }
    }
}
