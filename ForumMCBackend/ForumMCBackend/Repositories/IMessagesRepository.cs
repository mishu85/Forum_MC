using ForumMCBackend.Models;

namespace ForumMCBackend.Repositories
{
    public interface IMessagesRepository
    {
        Message? GetByID(int id);
        Message? Add(Message message);
        List<Message> GetReplies(int id);
        Message? Patch(Message message);
        List<Message> GetByTopicID(int id);
    }
}
