using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SmartGearWeb.Models;

namespace SmartGearWeb.Data
{
    // Now inherits IdentityDbContext instead of plain DbContext, so EF Core
    // also manages the Identity tables (users, roles, claims, logins, etc.)
    // alongside our own Products and Categories tables.
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products => Set<Product>();
        public DbSet<Category> Categories => Set<Category>();
    }
}
