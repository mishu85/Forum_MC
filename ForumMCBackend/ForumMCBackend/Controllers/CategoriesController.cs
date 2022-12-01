using ForumMCBackend.Db;
using ForumMCBackend.Models;
using ForumMCBackend.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using Microsoft.EntityFrameworkCore;

namespace ForumMCBackend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly ILogger<CategoriesController> _logger;
        private readonly MyDbContext _dbContext;

        public CategoriesController(ILogger<CategoriesController> logger, MyDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        [HttpGet]
        public ActionResult<List<Category>> Get()
        {
            var categories = _dbContext.Categories.ToList();
            return categories;
        }

        [HttpGet("{id}/topics")]
        public ActionResult<List<Topic>> GetTopics([FromHeader] string? authorization, int id)
        {
            var listOfTopics = _dbContext.Topics.Include(topic => topic.CreatedBy)
                .Where(topic => topic.Category.Id == id)
               .ToList();

            var requestFrom = new Account();
            if (authorization != null)
            {
                if (!AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
                {
                    return new ObjectResult(null) { StatusCode = StatusCodes.Status500InternalServerError };
                }
                requestFrom = AuthenticationUtils.GetAccountFromToken(headerValue.Parameter, _dbContext);
            }

            var topics = new List<Topic>();

            foreach (var dbTopic in listOfTopics)
            {
                if (dbTopic.IsHidden &&
                    (requestFrom.Role == AccountRoles.ADMIN ||
                    requestFrom.Role == AccountRoles.MODERATOR))
                {
                    topics.Add(dbTopic);
                }
                else if (!dbTopic.IsHidden)
                {
                    topics.Add(dbTopic);
                }
            }

            return new ObjectResult(topics);
        }
    }
}