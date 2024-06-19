using ChatApiApplication.Enums;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ChatApiApplication.Model
{
    public class Messages
    {
        [Key]
        public Guid MessageId { get; set; }
        public Guid? ParentMessageId { get; set; }
        [ForeignKey("Group")]
        public int? GroupId { get; set; }
        //[ForeignKey("SenderUser")]
        public Guid SenderId { get; set; }
        //[ForeignKey("RecieverUser")]
        public Guid ReceiverId { get; set;}
        [Required]
        [StringLength(1000, MinimumLength = 2)]
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }
        /*[JsonIgnore]
        public virtual ChatUsers RecieverUser { get; set; }
        public virtual ChatUsers SenderUser { get; set; }*/
        public Group Group { get; set; }
    }
}
