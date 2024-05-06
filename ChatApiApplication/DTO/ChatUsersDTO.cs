using System.ComponentModel.DataAnnotations;

namespace ChatApiApplication.DTO
{
    public class ChatUsersDTO
    {
        [Key]
        [Required]
        public Guid UserId { get; set; }

        [Required(ErrorMessage = "Full Name is required")]
        [RegularExpression(@"^[a-zA-Z\s]*$", ErrorMessage = "UserName must contain only letters and spaces")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [RegularExpression(@"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$", ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
