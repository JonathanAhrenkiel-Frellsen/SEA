using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Survey.Domain.Entities
{
    public class User
    {
        [Key]
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string UserPassword { get; set; }
        [ForeignKey("UserType")]
        public int UserTypeId { get; set; }
        [JsonIgnore]
        public virtual UserType? UserType { get; set; }
        [JsonIgnore]
        public virtual ICollection<DesignedSurvey>? Surveys { get; set; }
        [JsonIgnore]
        public virtual ICollection<SurveyCompletion>? SurveyCompletions { get; set; } 

        public User()
        {
            Surveys = new HashSet<DesignedSurvey>();
            SurveyCompletions = new HashSet<SurveyCompletion>();
        }
    }
}
