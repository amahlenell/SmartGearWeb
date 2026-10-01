using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SmartGearWeb.Data;
using SmartGearWeb.Models;

namespace SmartGearWeb.Services
{
    public interface IProductRepository
    {
        IEnumerable<Product> GetAll();
        Product? GetById(int id);
        void Add(Product product);
        void Update(Product product);
        void Delete(int id);
    }

    // Backed by SQL Server via EF Core, with an in-memory cache in front of
    // the full product list — the most frequently hit read in the app.
    public class ProductRepository : IProductRepository
    {
        private const string ProductListCacheKey = "ProductList";

        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _cache;

        public ProductRepository(ApplicationDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public IEnumerable<Product> GetAll()
        {
            // GetOrCreate: returns the cached list if present, otherwise runs
            // the database query once and caches the result for future calls.
            return _cache.GetOrCreate(ProductListCacheKey, entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1);
                return _context.Products.Include(p => p.Category).ToList();
            })!;
        }

        // Single-product lookups aren't cached — they're cheap, indexed
        // primary-key reads, and caching every id would add complexity
        // (and memory usage) for little benefit.
        public Product? GetById(int id) =>
            _context.Products.Include(p => p.Category).FirstOrDefault(p => p.Id == id);

        public void Add(Product product)
        {
            _context.Products.Add(product);
            _context.SaveChanges();
            InvalidateProductListCache();
        }

        public void Update(Product product)
        {
            _context.Products.Update(product);
            _context.SaveChanges();
            InvalidateProductListCache();
        }

        public void Delete(int id)
        {
            var product = _context.Products.Find(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                _context.SaveChanges();
                InvalidateProductListCache();
            }
        }

        // Any write must clear the cache — otherwise GetAll() would keep
        // serving stale data for up to a minute after a change.
        private void InvalidateProductListCache() => _cache.Remove(ProductListCacheKey);
    }
}
