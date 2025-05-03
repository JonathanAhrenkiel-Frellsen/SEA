using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Survey.Domain.Entities
{
    public class SurveyType
    {
        [JsonIgnore]
        public int SurveyTypeId { get; set; }
        public string SurveyTypeName { get; set; } = string.Empty;
        [JsonIgnore]
        public virtual ICollection<DesignedSurvey>? DesignedSurveys { get; set; }

        public SurveyType()
        {
            DesignedSurveys = [];
        }
    }
}
