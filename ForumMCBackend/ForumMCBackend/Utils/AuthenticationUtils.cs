using ForumMCBackend.Db;
using ForumMCBackend.Models;
using System.IdentityModel.Tokens.Jwt;

namespace ForumMCBackend.Utils
{
    public class AuthenticationUtils
    {
        public static Account GetAccountFromToken(string token, SQLiteContext dbContext)
        {
            var stream = token;
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(stream);
            var tokenS = jsonToken as JwtSecurityToken;

            var accountId = tokenS.Claims.First(claim => claim.Type == "UserId").Value;

            var account = dbContext.Accounts.SingleOrDefault(entity => entity.Id == int.Parse(accountId));
            return account;
        }
    }
}
