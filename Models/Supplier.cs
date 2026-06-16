using System.ComponentModel.DataAnnotations;

namespace CommerceSystemAPI.Models
{
    public class Supplier
    {
        [Key]
        public int SupplierId { get; set; }

        [Required]
        [MaxLength(100)]
        public string SupplierName { get; set; }

        [EmailAddress]
        public string? ContactEmail { get; set; }

        [Phone]
        public string? Phone { get; set; }

        public virtual ICollection<Product>? Products { get; set; }
    }
}
