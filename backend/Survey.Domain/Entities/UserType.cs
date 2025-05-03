using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Survey.Domain.Entities
{
    public class UserType
    {
        [Key]
        public int UserTypeId { get; set; }
        public string UserTypeName { get; set; } = string.Empty;
        [JsonIgnore]
        public virtual ICollection<User>? Users { get; set; }

        public UserType()
        {
            Users = [];
        }
    }
}
