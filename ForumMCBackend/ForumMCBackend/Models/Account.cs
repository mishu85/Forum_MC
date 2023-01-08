using ForumMCBackend.Db;
using System.Security.Claims;

namespace ForumMCBackend.Models
{
    public enum AccountRoles
    {
        USER,
        MODERATOR,
        ADMIN,
    }

    public class Account : BaseEntity
    {
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public AccountRoles Role { get; set; } = AccountRoles.USER;
    
        static public bool IsInRoles(ClaimsPrincipal user, List<AccountRoles> roles)
        {
            foreach(var role in roles)
            {
                if(user.IsInRole(role.ToString()))
                {
                    return true;
                }
            }

            return false;            
        }

        static public bool IsInRole(ClaimsPrincipal user, AccountRoles role)
        {
            return user.IsInRole(role.ToString());
        }
    }
}
