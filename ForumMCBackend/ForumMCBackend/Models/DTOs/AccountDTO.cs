namespace ForumMCBackend.Models.DTOs
{
    public class AccountDTO
    {
        public int Id { get; set; }
        public string? UserName { get; set; }
        public AccountRoles Role { get; set; } = AccountRoles.USER;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
