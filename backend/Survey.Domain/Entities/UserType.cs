using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Survey.Domain.Entities
{
    public class UserType
    {
        [Key]
        public int UserTypeId { get; set; }
        public string UserTypeName { get; set; }
        [JsonIgnore]
        public virtual ICollection<User>? Users { get; set; }

        public UserType()
        {
            Users = new HashSet<User>();
        }
    }
}
