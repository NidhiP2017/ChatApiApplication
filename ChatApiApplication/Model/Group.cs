using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
//using ChatApiApplication.Model;

namespace ChatApiApplication.Model
{
    public class Group
    {
        [Key]
        public int GroupId { get; set; }
        [Required(ErrorMessage = "Group name is required.")]
        public string GroupName { get; set; }

        [JsonIgnore]
        public virtual ICollection<Messages> Messages { get; set; }
        public virtual ICollection<GroupMembers> GroupMembers { get; set; } = new List<GroupMembers>();
    }
}
