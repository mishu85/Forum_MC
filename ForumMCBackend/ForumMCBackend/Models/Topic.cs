using ForumMCBackend.Db;

namespace ForumMCBackend.Models
{
    public class Topic : BaseEntity
    {
        public string? Title { get; set; }
        public Account? CreatedBy { get; set; }
        public Category? Category { get; set; }
        public bool IsHidden { get; set; } = false;
    }
}
