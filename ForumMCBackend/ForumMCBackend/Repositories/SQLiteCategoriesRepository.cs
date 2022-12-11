using ForumMCBackend.Db;
using ForumMCBackend.Models;

namespace ForumMCBackend.Repositories
{
    public class SQLiteCategoriesRepository : ICategoriesRepository
    {
        SQLiteContext _context = new();

        public List<Category> getAll()
        {
            var categories = _context?.Categories?.ToList() ?? new List<Category>();
            return categories;
        }
    }
}
