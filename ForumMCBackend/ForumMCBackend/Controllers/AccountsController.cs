using ForumMCBackend.Db;
using ForumMCBackend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ForumMCBackend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountsController : ControllerBase
    {
        private readonly ILogger<AccountsController> _logger;
        private readonly MyDbContext _dbContext;
        public IConfiguration _configuration;

        public AccountsController(ILogger<AccountsController> logger, IConfiguration config, MyDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
            _configuration = config;
        }

        [HttpPost("login")]
        public ActionResult<string> Login(Account request)
        {
            var account = _dbContext.Accounts.SingleOrDefault(entity => entity.UserName == request.UserName);
            if (account == null)
            {
                return new ObjectResult(null) { StatusCode = StatusCodes.Status404NotFound };
            }
            if (!BCrypt.Net.BCrypt.Verify(request.Password, account.Password))
            {
                return new ObjectResult(null) { StatusCode = StatusCodes.Status400BadRequest };
            }

            //create claims details based on the user information
            var claims = new[] {
                        new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                        new Claim("UserId", account.Id.ToString()),
                        new Claim("UserName", account.UserName),
                    };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.UtcNow.AddMinutes(10),
                signingCredentials: signIn);

            return new ObjectResult(new JwtSecurityTokenHandler().WriteToken(token));
        }

        [HttpPost("register")]
        public ActionResult<Account> Register(Account account)
        {
            var existingAccount = _dbContext.Accounts.SingleOrDefault(entity => entity.UserName == account.UserName);
            if (existingAccount != null)
            {
                return new ObjectResult(null) { StatusCode = StatusCodes.Status400BadRequest };
            }

            account.Password = BCrypt.Net.BCrypt.HashPassword(account.Password);
            _dbContext.Accounts.Add(account);
            _dbContext.SaveChanges();
            account.Password = null;
            return new ObjectResult(account) { StatusCode = StatusCodes.Status201Created };
        }
    }
}