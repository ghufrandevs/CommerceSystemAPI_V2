using System.ComponentModel.DataAnnotations;

namespace CommerceSystemAPI.DTOs
{
    public class CategoryUpdateDTO
    {
        [Required]
        [MaxLength(100)]
        public string CategoryName { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }
    }
}
