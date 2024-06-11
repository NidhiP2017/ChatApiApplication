using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ChatApiApplication.DTO
{
    public class MessagesDTO
    {
        [Key]
        public Guid MessageId { get; set; }
        [ForeignKey("UserId")]
        public Guid SenderId { get; set; }

        public Guid? ParentMessageId { get; set; }

        [ForeignKey("UserId")]
        public Guid ReceiverId { get; set; }
        [ForeignKey("GroupId")]
        public int? GroupId { get; set; }
        [Required]
        [StringLength(1000, MinimumLength = 2)]
        public string Content { get; set; }        
        public DateTime Timestamp { get; set; }
    }
}
