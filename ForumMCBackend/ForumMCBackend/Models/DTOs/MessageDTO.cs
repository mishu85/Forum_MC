namespace ForumMCBackend.Models.DTOs
{
    public class MessageDTO
    {
        public int Id { get; set; }
        public string? BodyText { get; set; }
        public AccountDTO? CreatedBy { get; set; }
        public TopicDTO? Topic { get; set; }
        public DateTime GoesLive { get; set; } = DateTime.UtcNow;
        public bool IsHidden { get; set; } = false;
        public MessageDTO? InReplyTo { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
