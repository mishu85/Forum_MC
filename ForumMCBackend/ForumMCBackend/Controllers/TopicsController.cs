using ForumMCBackend.Db;
using ForumMCBackend.Models;
using ForumMCBackend.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace ForumMCBackend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TopicsController : ControllerBase
    {
        private readonly ILogger<TopicsController> _logger;
        private readonly MyDbContext _dbContext;

        public TopicsController(ILogger<TopicsController> logger, MyDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        [Authorize]
        [HttpPost]
        public ActionResult<Topic> Post([FromHeader] string authorization, Topic topic)
        {
            if(topic.Title == null)
            {
                return new ObjectResult(null) { StatusCode = StatusCodes.Status400BadRequest};
            }


            if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
            {
                var requestFrom = AuthenticationUtils.GetAccountFromToken(headerValue.Parameter, _dbContext);
                topic.CreatedBy = requestFrom;
            }
            else {
                return new ObjectResult(null) { StatusCode = StatusCodes.Status500InternalServerError };
            }

            _dbContext.Topics.Add(topic);
            _dbContext.SaveChanges();
            return new ObjectResult(topic) { StatusCode = StatusCodes.Status201Created };
        }
    }
}