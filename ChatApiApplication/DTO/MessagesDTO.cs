using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using ChatApiApplication.Model;

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
        public Group Group { get; set; }
        public ChatUsers User { get; set; }
    }

    public class MessageGroupDTO
    {
        public int? GroupId { get; set; }
        public string GroupName { get; set; }
        public string UserName { get; set; }
        public string  MessageContent { get; set; }
    }
}
