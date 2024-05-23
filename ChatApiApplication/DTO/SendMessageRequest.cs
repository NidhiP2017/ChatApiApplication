using System.ComponentModel.DataAnnotations;

namespace ChatApiApplication.DTO
{
    public class SendMessageRequest
    {
        [Required]
        public Guid SenderId { get; set; }

        [Required]
        public Guid ReceiverId { get; set; }

        [Required]
        public string Content { get; set; }
    }
}
