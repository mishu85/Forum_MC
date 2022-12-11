using ForumMCBackend.Models;

namespace ForumMCBackend.Repositories
{
    public interface IAccountsRepository
    {
        Account? getByID(int id);
        List<Account> getAll();
    }
}
