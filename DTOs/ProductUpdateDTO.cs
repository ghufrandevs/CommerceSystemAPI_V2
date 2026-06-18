using System.ComponentModel.DataAnnotations;

namespace CommerceSystemAPI.DTOs
{
    public class ProductUpdateDTO
    {
        [Required]
        public string ProductName { get; set; }

        public string? Description { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int Stock { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        public int SupplierId { get; set; }
    }
}
