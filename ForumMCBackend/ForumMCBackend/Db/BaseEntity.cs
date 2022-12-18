using System.ComponentModel.DataAnnotations;

namespace ForumMCBackend.Db
{
    public class BaseEntity
    {
        [Key]
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
