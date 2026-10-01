using SmartGearWeb.Data;
using SmartGearWeb.Models;

namespace SmartGearWeb.Services
{
    public interface ICategoryRepository
    {
        IEnumerable<Category> GetAll();
        Category? GetById(int id);
    }

    // Now backed by SQL Server via EF Core instead of an in-memory list.
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public CategoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Category> GetAll() => _context.Categories.ToList();

        public Category? GetById(int id) => _context.Categories.Find(id);
    }
}
