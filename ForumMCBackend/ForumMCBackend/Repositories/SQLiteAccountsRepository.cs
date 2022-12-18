using ForumMCBackend.Db;
using ForumMCBackend.Models;

namespace ForumMCBackend.Repositories
{
    public class SQLiteAccountsRepository : IAccountsRepository
    {
        readonly SQLiteContext _context = new();

        public Account? Add(Account account)
        {
            _context.Accounts?.Add(account);
            _context.SaveChanges();
            return account;
        }

        public List<Account> GetAll()
        {
            var accounts = _context?.Accounts?.ToList() ?? new List<Account>();
            return accounts;
        }

        public Account? GetByID(int id)
        {
            var account = _context?.Accounts?.FirstOrDefault(account => account.Id == id);
            return account;
        }

        public Account? GetByUserName(string? userName)
        {
            if (userName == null) return null;
            var account = _context?.Accounts?.SingleOrDefault(entity => entity.UserName == userName);
            return account;
        }

        public Account? Patch(Account account)
        {
            var dbAccount = GetByID(account.Id);
            if (dbAccount == null) return null;
            dbAccount.Role = account.Role;
            _context.SaveChanges();
            return dbAccount;
        }
    }
}
