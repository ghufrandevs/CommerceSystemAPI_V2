using System.ComponentModel.DataAnnotations;

namespace CommerceSystemAPI.DTOs
{
    public class SupplierCreateDTO
    {
        [Required]
        [MaxLength(100)]
        public string SupplierName { get; set; }

        [Required]
        [EmailAddress]
        public string? ContactEmail { get; set; }

        [Required]
        public string? Phone { get; set; }
    }
}

