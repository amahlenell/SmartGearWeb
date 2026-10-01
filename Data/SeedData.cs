using Microsoft.EntityFrameworkCore;
using SmartGearWeb.Models;

namespace SmartGearWeb.Data
{
    // Applies pending migrations and seeds starter data on first run,
    // replacing the hard-coded lists that used to live in the repositories.
    public static class SeedData
    {
        public static void EnsurePopulated(IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            // Creates the database and applies any pending migrations
            context.Database.Migrate();

            if (!context.Categories.Any())
            {
                context.Categories.AddRange(
                    new Category { Name = "Jerseys", Description = "Customizable team jerseys" },
                    new Category { Name = "Shoes", Description = "Performance footwear" },
                    new Category { Name = "Accessories", Description = "Bottles, bags, and extras" }
                );
                context.SaveChanges();
            }

            if (!context.Products.Any())
            {
                var jerseys = context.Categories.First(c => c.Name == "Jerseys");
                var shoes = context.Categories.First(c => c.Name == "Shoes");
                var accessories = context.Categories.First(c => c.Name == "Accessories");

                context.Products.AddRange(
                    new Product { Name = "Rugby Jersey - Medium", Description = "Customizable team jersey", Price = 899.99m, CategoryId = jerseys.Id, StockQuantity = 25 },
                    new Product { Name = "Running Shoes - Size 9", Description = "Lightweight performance trainer", Price = 1299.00m, CategoryId = shoes.Id, StockQuantity = 12 },
                    new Product { Name = "Water Bottle", Description = "1L insulated bottle", Price = 199.50m, CategoryId = accessories.Id, StockQuantity = 3 }
                );
                context.SaveChanges();
            }
        }
    }
}
