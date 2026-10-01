using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartGearWeb.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Product name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(0.01, 100000, ErrorMessage = "Price must be between R0.01 and R100 000")]
        [Column(TypeName = "decimal(18,2)")]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Please select a category")]
        public int CategoryId { get; set; }

        // Navigation property: not bound from form input directly,
        // populated by the repository/EF Core so views can show Category.Name.
        public Category? Category { get; set; }

        [Range(0, 10000, ErrorMessage = "Stock quantity must be 0 or more")]
        public int StockQuantity { get; set; }

        // URL of the product's image, stored in cloud object storage
        // (Cloudinary) rather than on the web server's own disk.
        public string? ImageUrl { get; set; }

        // ---- Business logic ----

        // Price including South African standard VAT (15%)
        public decimal GetPriceIncludingVat(decimal vatRate = 0.15m)
        {
            return Math.Round(Price * (1 + vatRate), 2);
        }

        // Applies a percentage discount safely, guarding against invalid input
        public decimal GetDiscountedPrice(decimal discountPercentage)
        {
            if (discountPercentage is < 0 or > 100)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(discountPercentage), "Discount must be between 0 and 100.");
            }

            return Math.Round(Price - (Price * discountPercentage / 100), 2);
        }

        // Simple business rule used to drive the stock badge in the views
        public bool IsLowStock => StockQuantity <= 5;
    }
}
