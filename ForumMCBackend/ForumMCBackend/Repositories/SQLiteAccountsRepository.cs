using ForumMCBackend.Db;
using ForumMCBackend.Models;

namespace ForumMCBackend.Repositories
{
    public class SQLiteAccountsRepository : IAccountsRepository
    {
        SQLiteContext _context = new();

        public List<Account> getAll()
        {
            var accounts = _context?.Accounts?.ToList() ?? new List<Account>();
            return accounts;
        }

        public Account? getByID(int id)
        {
            var account = _context?.Accounts?.FirstOrDefault(account => account.Id == id);
            return account;
        }
    }
}
