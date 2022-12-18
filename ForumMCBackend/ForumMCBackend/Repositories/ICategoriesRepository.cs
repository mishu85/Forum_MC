using ForumMCBackend.Models;

namespace ForumMCBackend.Repositories
{
    public interface ICategoriesRepository
    {
        List<Category> GetAll();

        Category? GetByID(int id);
    }
}
