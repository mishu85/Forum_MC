using ForumMCBackend.Models;

namespace ForumMCBackend.Repositories
{
    public interface ITopicsRepository
    {
        Topic? GetByID(int id);
        List<Topic> GetByCategoryID(int id);
        Topic? Add(Topic topic);
        List<Topic> GetLatestTenTopics();
        Topic? Patch(Topic topic);
    }
}
