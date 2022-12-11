using ForumMCBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace ForumMCBackend.Db
{
    public class SQLiteContext : DbContext
    {
		public DbSet<Account>? Accounts { get; set; }
		public DbSet<Category>? Categories { get; set; }
		public DbSet<Topic>? Topics { get; set; }
		public DbSet<Message>? Messages { get; set; }


		protected override void OnConfiguring(DbContextOptionsBuilder builder)
		{
			builder.UseSqlite("Filename=localdb.sqlite;");
			base.OnConfiguring(builder);
		}

		public override int SaveChanges()
		{
			DateTime currentDateTime = DateTime.UtcNow;
			var entities = ChangeTracker.Entries<BaseEntity>();

			// handle newly added entities
			foreach (var entity in entities.Where(e => (e.State == EntityState.Added)))
			{
				// set the CreatedAt field to the current date&time
				entity.Entity.CreatedAt = currentDateTime;
				entity.Entity.UpdatedAt = currentDateTime;
			}

			// handle updated entities
			foreach (var entity in entities.Where(e => (e.State == EntityState.Modified)))
			{
				entity.Entity.UpdatedAt = currentDateTime;
			}

			// to the actual saving of the data
			return base.SaveChanges();
		}
	}
}
