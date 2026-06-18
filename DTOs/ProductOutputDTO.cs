namespace CommerceSystemAPI.DTOs
{
    public class ProductOutputDTO
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public string? Description { get; set; }

        public decimal Price { get; set; }

        public int Stock { get; set; }

        public decimal OverallRating { get; set; }

        public string? CategoryName { get; set; }

        public string? SupplierName { get; set; }
    }
}
