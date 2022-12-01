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
            if (topic.Title == null || topic.Category == null)
            {
                return new ObjectResult(null) { StatusCode = StatusCodes.Status400BadRequest };
            }

            var category = _dbContext.Categories.SingleOrDefault(entity => entity.Id == topic.Category.Id);
            if (category == null)
            {
                return new ObjectResult(null) { StatusCode = StatusCodes.Status400BadRequest };
            }
            else
            {
                topic.Category = category;
            }

            if (!AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
            {
                return new ObjectResult(null) { StatusCode = StatusCodes.Status500InternalServerError };
            }
            else
            {
                var requestFrom = AuthenticationUtils.GetAccountFromToken(headerValue.Parameter, _dbContext);
                topic.CreatedBy = requestFrom;
            }

            _dbContext.Topics.Add(topic);
            _dbContext.SaveChanges();
            topic.CreatedBy.Password = null;
            return new ObjectResult(topic) { StatusCode = StatusCodes.Status201Created };
        }

        [HttpGet]
        public ActionResult<List<Topic>> GetLatestTenTopics([FromHeader] string? authorization)
        {
            var listOfTopics = _dbContext.Topics.Include(topic => topic.CreatedBy)
                .OrderByDescending(topic => topic.CreatedAt)
                .Take(10)
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

        [Authorize]
        [HttpPatch]
        public ActionResult<Topic> PatchTopic([FromHeader] string authorization, Topic topic)
        {
            var dbTopic = _dbContext.Topics.SingleOrDefault(entity => entity.Id == topic.Id);
            if (dbTopic == null)
            {
                return new ObjectResult(null) { StatusCode = StatusCodes.Status404NotFound };
            }

            if (!AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
            {
                return new ObjectResult(null) { StatusCode = StatusCodes.Status500InternalServerError };
            }
            else
            {
                var requestFrom = AuthenticationUtils.GetAccountFromToken(headerValue.Parameter, _dbContext);
                if (requestFrom.Role != AccountRoles.ADMIN && requestFrom.Role != AccountRoles.MODERATOR)
                {
                    return new ObjectResult(null) { StatusCode = StatusCodes.Status401Unauthorized };
                }
            }

            dbTopic.IsHidden = topic.IsHidden;
            _dbContext.SaveChanges();

            return new ObjectResult(dbTopic);
        }

        [HttpGet("{topicId}/messages")]
        public ActionResult<List<Message>> GetTopicMessages([FromHeader] string? authorization, int topicId)
        {
            var messagesOfTopic = _dbContext.Messages.Include(message => message.CreatedBy)
               .Include(message => message.Topic)
                .Where(e => (e.Topic.Id == topicId) && (e.InReplyTo == null))
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

            var messages = new List<Message>();
            var now = DateTime.UtcNow;

            foreach (var message in messagesOfTopic)
            {
                if (requestFrom.Role == AccountRoles.ADMIN ||
                    requestFrom.Role == AccountRoles.MODERATOR ||
                    message.CreatedBy.Id == requestFrom.Id)
                {
                    message.CreatedBy.Password = null;
                    message.Topic.CreatedBy.Password = null;
                    messages.Add(message);
                }
                else
                {
                    if (message.IsHidden)
                    {
                        message.BodyText = "";
                    }
                    if (now > message.GoesLive)
                    {
                        message.CreatedBy.Password = null;
                        message.Topic.CreatedBy.Password = null;
                        messages.Add(message);
                    }
                }
            }

            return messages;
        }
    }
}