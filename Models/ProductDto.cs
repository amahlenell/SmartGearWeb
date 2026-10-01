using System.ComponentModel.DataAnnotations;

namespace SmartGearWeb.Models
{
    // Data Transfer Object: what the API actually sends/receives over JSON.
    // Kept separate from the Product entity so the API's shape doesn't
    // depend on EF Core's navigation properties (avoiding serialization
    // cycles) and can evolve independently of the database schema.
    public class ProductDto
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [Range(0.01, 100000)]
        public decimal Price { get; set; }

        [Required]
        public int CategoryId { get; set; }

        public string CategoryName { get; set; } = string.Empty;

        [Range(0, 10000)]
        public int StockQuantity { get; set; }
    }
}
