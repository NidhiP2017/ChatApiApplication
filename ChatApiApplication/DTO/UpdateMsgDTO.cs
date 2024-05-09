using System.ComponentModel.DataAnnotations;

namespace ChatApiApplication.DTO
{
    public class UpdateMsgDTO
    {
        [Key]
        public Guid MessageId { get; set; }

        [Required]
        [StringLength(1000, MinimumLength = 2)]
        public string Content { get; set; }

    }
}
