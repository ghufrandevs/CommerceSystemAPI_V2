using System.ComponentModel.DataAnnotations;

namespace CommerceSystemAPI.DTOs
{
    public class UserUpdateDTO
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        public string UserEmail { get; set; }

        [Required]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\d).{8,}$")]
        public string UserPassword { get; set; }

        [Required]
        public string UserPhone { get; set; }

        public string Role { get; set; }

        public bool IsActive { get; set; }
    }
}
