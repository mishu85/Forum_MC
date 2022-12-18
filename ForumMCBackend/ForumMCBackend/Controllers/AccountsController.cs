using ForumMCBackend.Models;
using ForumMCBackend.Repositories;
using Microsoft.AspNetCore.Authorization;
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
        private readonly IConfiguration _configuration;
        private readonly IAccountsRepository _accountsRepository;

        public AccountsController(IConfiguration config, IAccountsRepository accountsRepository)
        {
            _configuration = config;
            _accountsRepository = accountsRepository;
        }

        [HttpPost("login")]
        public ActionResult<string> Login(Account request)
        {
            var account = _accountsRepository.GetByUserName(request.UserName);

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
                        new Claim(JwtRegisteredClaimNames.Name, account.Id.ToString()),
                    };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.UtcNow.AddDays(30),
                signingCredentials: signIn);

            return new ObjectResult(new JwtSecurityTokenHandler().WriteToken(token));
        }

        [HttpPost("register")]
        public ActionResult<Account> Register(Account account)
        {
            var existingAccount = _accountsRepository.GetByUserName(account.UserName);
            if (existingAccount != null)
            {
                return new ObjectResult(null) { StatusCode = StatusCodes.Status400BadRequest };
            }

            account.Password = BCrypt.Net.BCrypt.HashPassword(account.Password);
            account.Role = AccountRoles.USER;
            var result = _accountsRepository.Add(account);
            result.Password = null;
            return new ObjectResult(result) { StatusCode = StatusCodes.Status201Created };
        }

        [Authorize]
        [HttpGet]
        public ActionResult<List<Account>> Get()
        {
            var requestFromId = HttpContext.User.Identity?.Name ?? "0";
            var requestFrom = _accountsRepository.GetByID(int.Parse(requestFromId));

            if (requestFrom?.Role != AccountRoles.ADMIN)
            {
                return new ObjectResult(null) { StatusCode = StatusCodes.Status401Unauthorized };
            }

            var accounts = _accountsRepository.GetAll();
            return accounts;
        }

        [Authorize]
        [HttpPatch]
        public ActionResult<Account> Patch(Account account)
        {
            var dbAccount = _accountsRepository.GetByID(account.Id);
            if (dbAccount == null)
            {
                return new ObjectResult(null) { StatusCode = StatusCodes.Status404NotFound };
            }

            if (!Enum.IsDefined(typeof(AccountRoles), account.Role))
            {
                return new ObjectResult(null) { StatusCode = StatusCodes.Status400BadRequest };
            }

            var requestFromId = HttpContext.User.Identity?.Name ?? "0";
            var requestFrom = _accountsRepository.GetByID(int.Parse(requestFromId)) ?? new Account();
            if (requestFrom.Role != AccountRoles.ADMIN)
            {
                return new ObjectResult(null) { StatusCode = StatusCodes.Status401Unauthorized };
            }

            var result = _accountsRepository.Patch(account);

            return new ObjectResult(result);
        }
    }
}