using ApplicationCore.Entities;

namespace ForumMCBackend.Repositories
{
    public interface IAccountsRepository
    {
        Account? GetByID(int id);
        List<Account> GetAll();
        Account? GetByUserName(string? userName);
        Account? Add(Account account);
        Account? Patch(Account account);
    }
}
