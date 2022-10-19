using ForumMCBackend.Db;
using ForumMCBackend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using ForumMCBackend.Utils;


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
                var InReplyTo = _dbContext.Messages.SingleOrDefault(entity => entity.Id == message.InReplyTo.Id);
                if (InReplyTo == null)
                {
                    return new ObjectResult(null) { StatusCode = StatusCodes.Status400BadRequest };
                }
                else
                {
                    message.InReplyTo = InReplyTo;
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
    }
}