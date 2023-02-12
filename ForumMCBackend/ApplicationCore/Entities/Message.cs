

using Ardalis.GuardClauses;

namespace ApplicationCore.Entities
{
    public class Message : BaseEntity
    {
        #pragma warning disable CS8618 // Required by Entity Framework
        private Message() { }

        public Message(string bodyText, int createdById, int topicId, DateTime? goesLive, bool isHidden, int? inReplyToId)
        {
            BodyText = bodyText;
            CreatedById = createdById;
            TopicId = topicId;
            GoesLive = goesLive ?? DateTime.UtcNow;
            IsHidden = isHidden;
            InReplyToId = inReplyToId;
        }

        public string BodyText { get; private set; }
        public int CreatedById { get; private set; }
        public Account? CreatedBy { get; private set; }
        public int TopicId { get; private set; }
        public Topic? Topic { get; private set; }
        public DateTime? GoesLive { get; private set; } = DateTime.UtcNow;
        public bool IsHidden { get; private set; }
        public int? InReplyToId { get; private set; }
        public Message? InReplyTo { get; private set; }

        public void UpdateCreatedBy(int createdById)
        {
            Guard.Against.Zero(createdById, nameof(createdById));
            CreatedById = createdById;
        }

        public void UpdateBodyText(string bodyText)
        {
            BodyText = bodyText;
        }

        public void UpdateIsHidden(bool isHidden)
        {
            IsHidden = isHidden;
        }
    }
}
