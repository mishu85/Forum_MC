using ForumMCBackend.Db;

namespace ForumMCBackend.Models
{
    public class Account : BaseEntity
    {
        public string UserName { get; set; }
        public string? Password { get; set; }
    }
}
