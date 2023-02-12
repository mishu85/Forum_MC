namespace ApplicationCore.Entities.DTOs
{
    public class AccountDTO
    {
        public int Id { get; set; }
        public string? UserName { get; set; }
        public AccountRoles Role { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
