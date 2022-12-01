using ForumMCBackend.Db;

namespace ForumMCBackend.Models
{
    public enum AccountRoles
    {
        USER,
        MODERATOR,
        ADMIN,
    }

    public class Account : BaseEntity
    {
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public AccountRoles Role { get; set; } = AccountRoles.USER;
    }
}
