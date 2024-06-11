using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ChatApiApplication.Model
{
    public class GroupMembers
    {
        [Key]
        public int MemberId { get; set; } 
        [ForeignKey("UserId")]
        public Guid UserId { get; set; }
        [ForeignKey("GroupId")]
        public int GroupId { get; set; }
        public DateTime JoinTime { get; set; }

        public bool IncludePreviousChats { get; set; } = true;
        [RegularExpression(@"^(All|None|([1-9][0-9]?|1000))$", ErrorMessage = "Please enter a valid value for Number of Days")]
        public string NumOfDays { get; set; } = "All";

        [JsonIgnore]
        public ChatUsers User { get; set; }

        [JsonIgnore]
        public Group Group { get; set; }
    }
}
