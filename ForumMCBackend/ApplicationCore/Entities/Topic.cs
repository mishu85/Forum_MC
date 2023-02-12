

using Ardalis.GuardClauses;

namespace ApplicationCore.Entities
{
    public class Topic : BaseEntity
    {
        public Topic(string title, int createdById, int categoryId, bool isHidden)
        {
            Title = title;
            CreatedById = createdById;
            CategoryId = categoryId;
            IsHidden = isHidden;
        }

        public string Title { get; private set; }
        public int CreatedById { get; private set; }
        public Account? CreatedBy { get; private set; }
        public int CategoryId { get; private set; }
        public Category? Category { get; private set; }
        public bool IsHidden { get; private set; }

        public void UpdateCreatedBy(int createdById)
        {
            Guard.Against.Zero(createdById, nameof(createdById));
            CreatedById = createdById;
        }

        public void UpdateDetails(TopicDetails details)
        {
            Guard.Against.NullOrEmpty(details.Title, nameof(details.Title));

            Title = details.Title;
        }

        public void UpdateCategory(int categoryId)
        {
            Guard.Against.Zero(categoryId, nameof(categoryId));

            CategoryId = categoryId;
        }

        public void UpdateIsHidden(bool isHidden)
        {
            IsHidden = isHidden;
        }

        public readonly record struct TopicDetails
        {
            public string? Title { get; }

            public TopicDetails(string? title)
            {
                Title = title;
            }
        }
    }
}
