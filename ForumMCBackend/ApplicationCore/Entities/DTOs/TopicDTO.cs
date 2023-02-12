namespace ApplicationCore.Entities.DTOs
{
    public class TopicDTO
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public AccountDTO? CreatedBy { get; set; }
        public Category? Category { get; set; }
        public bool IsHidden { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
