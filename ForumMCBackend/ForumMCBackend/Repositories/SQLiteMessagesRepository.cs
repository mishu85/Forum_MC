using ForumMCBackend.Db;
using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;

namespace ForumMCBackend.Repositories
{
    public class SQLiteMessagesRepository : IMessagesRepository
    {
        readonly SQLiteContext _context = SQLiteContext.Default;

        public Message? Add(Message message)
        {
            _context.Messages?.Add(message);
            _context.SaveChanges();
            return message;
        }

        public Message? GetByID(int id)
        {
            var message = _context?.Messages?.Include(message => message.InReplyTo).FirstOrDefault(message => message.Id == id);
            return message;
        }

        public List<Message> GetByTopicID(int id)
        {
            var messagesOfTopic = _context.Messages?.Include(message => message.CreatedBy)
                           .Include(message => message.Topic)
                            .Where(e => ((e.Topic != null ? e.Topic.Id : 0) == id) && (e.InReplyTo == null))
                            .ToList();
            return messagesOfTopic ?? new List<Message>();
        }

        public List<Message> GetReplies(int id)
        {
            var repliesToMessage = _context?.Messages?.Include(message => message.CreatedBy)
                            .Include(message => message.Topic)
                            .Where(e => (e.InReplyTo != null ? e.InReplyTo.Id : 0) == id)
                            .ToList();
            return repliesToMessage ?? new List<Message>();
        }

        public Message? Patch(Message message)
        {
            var dbMessage = GetByID(message.Id);
            if (dbMessage == null) return null;
            dbMessage.UpdateIsHidden(message.IsHidden);
            _context.SaveChanges();
            return dbMessage;
        }
    }
}
