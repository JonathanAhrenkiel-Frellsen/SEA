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
    public class DesignedSurvey
    {
        [Key]
        public int SurveyId { get; set; }
        public string SurveyTitle { get; set; }
        public string SurveyDescription { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        [ForeignKey("User")]
        public int UserId { get; set; } //Experimenter Id
        [JsonIgnore]
        public virtual User? User { get; set; } //Experimenter
        public virtual ICollection<Questionnaire> Questionnaires { get; set; }

        public DesignedSurvey()
        {
            Questionnaires = new HashSet<Questionnaire>();
        }
    }
}
