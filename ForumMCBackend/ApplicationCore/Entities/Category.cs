

using Ardalis.GuardClauses;

namespace ApplicationCore.Entities
{
    public class Category : BaseEntity
    {
        public string Title { get; private set; }

        public Category(string title)
        {
            Title = title;
        }

        public void UpdateDetails(CategoryDetails details)
        {
            Guard.Against.NullOrEmpty(details.Title, nameof(details.Title));

            Title = details.Title;
        }
    }

    public readonly record struct CategoryDetails
    {
        public string? Title { get; }

        public CategoryDetails(string? title)
        {
            Title = title;
        }
    }
}
