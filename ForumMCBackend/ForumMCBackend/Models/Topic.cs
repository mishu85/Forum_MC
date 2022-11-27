using ForumMCBackend.Db;

namespace ForumMCBackend.Models
{
    public class Topic : BaseEntity
    {
        public string? Title { get; set; }
        public Account? CreatedBy { get; set; }
        public Category? Category { get; set; }
        public DateTime GoesLive { get; set; } = DateTime.UtcNow;
        public bool IsHidden { get; set; } = false;
    }
}
