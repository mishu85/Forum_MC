using ForumMCBackend.Db;

namespace ForumMCBackend.Models
{
    public enum AccountRoles
    {
        USER,
        ADMIN,
        MODERATOR,
    }

    public class Account : BaseEntity
    {
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? Email { get; set; }
        public AccountRoles Role { get; set; }
    }
}
