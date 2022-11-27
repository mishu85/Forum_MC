using ForumMCBackend.Db;
using ForumMCBackend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using ForumMCBackend.Utils;
using Microsoft.EntityFrameworkCore;

namespace ForumMCBackend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MessagesController : ControllerBase
    {
        private readonly ILogger<MessagesController> _logger;
        private readonly MyDbContext _dbContext;

        public MessagesController(ILogger<MessagesController> logger, MyDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        [Authorize]
        [HttpPost]
        public ActionResult<Message> Post([FromHeader] string authorization, Message message)
        {
            if (message.BodyText == null)
            {
                return new ObjectResult(null) { StatusCode = StatusCodes.Status400BadRequest };
            }

            if (message.Topic == null)
            {

                return new ObjectResult(null) { StatusCode = StatusCodes.Status400BadRequest };
            }
            else
            {
                var Topic = _dbContext.Topics.SingleOrDefault(entity => entity.Id == message.Topic.Id);
                if (Topic == null)
                {
                    return new ObjectResult(null) { StatusCode = StatusCodes.Status400BadRequest };
                }
                else
                {
                    message.Topic = Topic;
                }
            }

            if (message.InReplyTo != null)
            {
                var InReplyTo = _dbContext.Messages.Include(message => message.InReplyTo).SingleOrDefault(entity => entity.Id == message.InReplyTo.Id);

                if (InReplyTo == null)
                {
                    return new ObjectResult(null) { StatusCode = StatusCodes.Status400BadRequest };
                }
                else
                {
                    if (InReplyTo.InReplyTo != null)
                    {
                        // no replies to replies
                        return new ObjectResult(null) { StatusCode = StatusCodes.Status400BadRequest };
                    }
                    var topic = _dbContext.Topics.SingleOrDefault(entity => entity.Id == InReplyTo.Topic.Id);
                    message.InReplyTo = InReplyTo;
                    message.Topic = topic;
                }
            }

            if (!AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
            {
                return new ObjectResult(null) { StatusCode = StatusCodes.Status500InternalServerError };
            }
            else
            {
                var requestFrom = AuthenticationUtils.GetAccountFromToken(headerValue.Parameter, _dbContext);
                message.CreatedBy = requestFrom;
                Console.WriteLine("check header auth");
            }

            _dbContext.Messages.Add(message);
            _dbContext.SaveChanges();
            message.CreatedBy.Password = null;
            return new ObjectResult(message) { StatusCode = StatusCodes.Status201Created };
        }

        [HttpGet]
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

        [HttpGet("replies")]
        public ActionResult<List<Message>> GetMessageReplies([FromHeader] string? authorization, int messageId)
        {
            var repliesToMessage = _dbContext.Messages.Include(message => message.CreatedBy)
                .Include(message => message.Topic)
                .Where(e => e.InReplyTo.Id == messageId)
                .ToList();

            Account requestFrom = new();
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
            foreach (var message in repliesToMessage)
            {
                if (requestFrom.Role == AccountRoles.ADMIN ||
                    requestFrom.Role == AccountRoles.MODERATOR ||
                    message.CreatedBy.Id == requestFrom.Id)
                {
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
                        messages.Add(message);
                    }
                }
            }

            return messages;
        }


        [Authorize]
        [HttpPatch]
        public ActionResult<Message> PatchMessage([FromHeader] string authorization, Message message)
        {
            var dbMessage = _dbContext.Messages.SingleOrDefault(entity => entity.Id == message.Id);
            if (dbMessage == null)
            {
                return new ObjectResult(null) { StatusCode = StatusCodes.Status400BadRequest };
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

            dbMessage.IsHidden = message.IsHidden;
            _dbContext.SaveChanges();

            return dbMessage;
        }

    }
}