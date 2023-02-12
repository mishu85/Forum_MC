
using Ardalis.GuardClauses;
using System.Security.Claims;

namespace ApplicationCore.Entities
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
        public AccountRoles Role { get; private set; } = AccountRoles.USER;

        public Account (string userName, string password, AccountRoles role)
        {
            UserName = userName;
            Password = password;
            Role = role;
        }

        public void UpdateRole(AccountRoles role)
        {
            Guard.Against.EnumOutOfRange(Role, nameof(Role));

            Role = role;
        }

    
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
