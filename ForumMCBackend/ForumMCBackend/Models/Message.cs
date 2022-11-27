using ForumMCBackend.Db;

namespace ForumMCBackend.Models
{
    public class Message : BaseEntity
    {
        public string? BodyText { get; set; }
        public Account? CreatedBy { get; set; }
        public Topic? Topic { get; set; }
        public DateTime GoesLive { get; set; } = DateTime.UtcNow;
        public bool IsHidden { get; set; } = false;
        public Message? InReplyTo { get; set; } //null
    }
}
