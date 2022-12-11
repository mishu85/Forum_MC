using ForumMCBackend.Models;

namespace ForumMCBackend.Repositories
{
    public interface ICategoriesRepository
    {
        List<Category> getAll();
    }
}
