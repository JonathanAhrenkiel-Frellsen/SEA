using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Survey.Domain.Entities
{
    public class SurveyCompletionType
    {
        public int SurveyCompletionTypeId { get; set; }
        public string SurveyCompletionTypeName { get; set; } = string.Empty;
        public virtual ICollection<SurveyCompletion>? SurveyCompletions { get; set; }

        public SurveyCompletionType()
        {
            SurveyCompletions = [];
        }
    }
}
