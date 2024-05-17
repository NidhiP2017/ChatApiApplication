using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ChatApiApplication
    {
        public class Conversations
        {
            [Key]
            public Guid MessageId { get; set; }
            [ForeignKey("UserId")]
            public Guid SenderId { get; set; }
            [ForeignKey("UserId")]
            public Guid ReceiverId { get; set; }
            [Required]
            [StringLength(1000, MinimumLength = 2)]
            public string Content { get; set; }
            public DateTime Timestamp { get; set; }
        }
}
