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
        public string UserName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public string UserPassword { get; set; } = string.Empty;
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
            Surveys = [];
            SurveyCompletions = [];
        }
    }
}
