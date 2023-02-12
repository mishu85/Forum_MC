using ApplicationCore.Entities;
using ApplicationCore.Entities.DTOs;
using ApplicationCore.Exeptions;
using AutoMapper;
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
        private readonly IMapper _mapper;

        public AccountsController(IConfiguration config, IAccountsRepository accountsRepository, IMapper mapper)
        {
            _configuration = config;
            _accountsRepository = accountsRepository;
            _mapper = mapper;
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
                        new Claim(ClaimTypes.Role, account.Role.ToString()),
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
        public ActionResult<AccountDTO> Register(Account account)
        {
            var existingAccount = _accountsRepository.GetByUserName(account.UserName);
            if (existingAccount != null)
            {
                throw new DuplicateException($"An account with username {account.UserName} already exists");
            }

            account.Password = BCrypt.Net.BCrypt.HashPassword(account.Password);
            
            var result = _accountsRepository.Add(account);
            return new ObjectResult(
                _mapper.Map<AccountDTO>(result)
            ) { StatusCode = StatusCodes.Status201Created };
        }

        [Authorize(Roles = "ADMIN")]
        [HttpGet]
        public ActionResult<List<AccountDTO>> Get()
        {
            var accounts = _accountsRepository.GetAll().Select((account) =>
                _mapper.Map<AccountDTO>(account)
            ).ToList();
            return accounts;
        }

        [Authorize(Roles = "ADMIN")]
        [HttpPatch]
        public ActionResult<AccountDTO> Patch(Account account)
        {
            var result = _accountsRepository.Patch(account);
            return new ObjectResult(_mapper.Map<AccountDTO>(result));
        }
    }
}