using ForumMCBackend.Db;
using ForumMCBackend.Models;

namespace ForumMCBackend.Repositories
{
    public class SQLiteCategoriesRepository : ICategoriesRepository
    {
        readonly SQLiteContext _context = new();

        public List<Category> GetAll()
        {
            var categories = _context?.Categories?.ToList() ?? new List<Category>();
            return categories;
        }

        public Category? GetByID(int id)
        {
            var category = _context?.Categories?.FirstOrDefault(category => category.Id == id);
            return category;
        }
    }
}
