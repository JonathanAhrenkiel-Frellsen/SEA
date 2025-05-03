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
        public string SurveyTitle { get; set; } = string.Empty;
        public string SurveyDescription { get; set; } = string.Empty;
        public DateTime StartDate { get; set; } = DateTime.Now;
        public DateTime EndDate { get; set; } = DateTime.Now;
        [ForeignKey("SurveyType")]
        public int SurveyTypeId { get; set; }
        public virtual SurveyType? SurveyType { get; set; }
        public string? PrivateKey { get; set; }
        [ForeignKey("User")]
        public int UserId { get; set; } //Experimenter Id
        [JsonIgnore]
        public virtual User? User { get; set; } //Experimenter
        public virtual ICollection<Questionnaire> Questionnaires { get; set; }

        public DesignedSurvey()
        {
            Questionnaires = [];
        }
    }
}
