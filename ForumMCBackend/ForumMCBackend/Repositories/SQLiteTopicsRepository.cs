using ForumMCBackend.Db;
using ForumMCBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace ForumMCBackend.Repositories
{
    public class SQLiteTopicsRepository : ITopicsRepository
    {
        readonly SQLiteContext _context = new();

        public Topic? Add(Topic topic)
        {
            _context.Topics?.Add(topic);
            _context.SaveChanges();
            return topic;
        }

        public List<Topic> GetByCategoryID(int id)
        {
            var listOfTopics = _context?.Topics?.Include(topic => topic.CreatedBy)
                            .Where(topic => (topic.Category != null ? topic.Category.Id : 0) == id)
                           .ToList();
            return listOfTopics ?? new List<Topic>();
        }

        public Topic? GetByID(int id)
        {
            var topic = _context?.Topics?.FirstOrDefault(topic => topic.Id == id);
            return topic;
        }

        public List<Topic> GetLatestTenTopics()
        {
            var listOfTopics = _context.Topics?.Include(topic => topic.CreatedBy)
                .OrderByDescending(topic => topic.CreatedAt)
                .Take(10)
                .ToList();
            return listOfTopics ?? new List<Topic>();
        }

        public Topic? Patch(Topic topic)
        {
            var dbTopic = GetByID(topic.Id);
            if (dbTopic == null) return null;
            dbTopic.IsHidden = topic.IsHidden;
            _context.SaveChanges();
            return dbTopic;
        }
    }
}
