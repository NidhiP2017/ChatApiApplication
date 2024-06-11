using ChatApiApplication.Enums;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChatApiApplication.Model
{
    public class Messages
    {
        [Key]
        public Guid MessageId { get; set; }
        public Guid? ParentMessageId { get; set; }
        [ForeignKey("GroupId")]
        public int? GroupId { get; set; }
        [ForeignKey("UserId")]
        public Guid SenderId { get; set; }
        [ForeignKey("UserId")]
        public Guid ReceiverId { get; set;}
        [Required]
        [StringLength(1000, MinimumLength = 2)]
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }

    }
}
