using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Survey.Domain.Entities
{
    public class SurveyCompletion
    {
        [Key]
        public int SurveyCompletionId { get; set; }
        public DateTime SurveyCompletionDate { get; set; }
        [ForeignKey("DesignedSurvey")]
        public int SurveyId { get; set; }
        [JsonIgnore]
        public virtual DesignedSurvey? Survey { get; set; }
        [ForeignKey("User")]
        public int UserId { get; set; }
        [JsonIgnore]
        public virtual User? User { get; set; }
        [ForeignKey("SurveyCompletionType")]
        public int SurveyCompletionTypeId { get; set; }
        [JsonIgnore]
        public virtual SurveyCompletionType? SurveyCompletionType { get; set; }
        public virtual ICollection<SurveyAnswer> SurveyAnswers { get; set; }

        public SurveyCompletion()
        {
            SurveyAnswers = [];
        }
    }
}
