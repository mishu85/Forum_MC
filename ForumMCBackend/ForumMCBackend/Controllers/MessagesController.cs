using ForumMCBackend.Models;
using ForumMCBackend.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ForumMCBackend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MessagesController : ControllerBase
    {
        private readonly IMessagesRepository _messagesRepository;
        private readonly ITopicsRepository _topicsRepository;
        private readonly IAccountsRepository _accountsRepository;

        public MessagesController(IMessagesRepository messagesRepository, ITopicsRepository topicsRepository, IAccountsRepository accountsRepository)
        {
            _messagesRepository = messagesRepository;
            _topicsRepository = topicsRepository;
            _accountsRepository = accountsRepository;
        }

        [Authorize]
        [HttpPost]
        public ActionResult<Message> Post(Message message)
        {
            if (message.BodyText == null || message.Topic == null)
            {
                return new ObjectResult(null) { StatusCode = StatusCodes.Status400BadRequest };
            }

            var Topic = _topicsRepository.GetByID(message.Topic.Id);

            if (Topic == null)
            {
                return new ObjectResult(null) { StatusCode = StatusCodes.Status400BadRequest };
            }
            else
            {
                message.Topic = Topic;
            }

            if (message.InReplyTo != null)
            {
                var InReplyTo = _messagesRepository.GetByID(message.InReplyTo.Id);

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
                    message.InReplyTo = InReplyTo;
                }
            }

            var requestFromId = HttpContext.User.Identity?.Name ?? "0";
            var requestFrom = _accountsRepository.GetByID(int.Parse(requestFromId));
            message.CreatedBy = requestFrom;

            var result = _messagesRepository.Add(message);
            result.CreatedBy.Password = null;
            return new ObjectResult(result) { StatusCode = StatusCodes.Status201Created };
        }

        [HttpGet("{messageId}/replies")]
        public ActionResult<List<Message>> GetMessageReplies(int messageId)
        {
            var repliesToMessage = _messagesRepository.GetReplies(messageId);
            var requestFromId = HttpContext.User.Identity?.Name ?? "0";

            var messages = new List<Message>();
            var now = DateTime.UtcNow;
            foreach (var message in repliesToMessage)
            {
                if (Account.IsInRoles(HttpContext.User, new List<AccountRoles> { AccountRoles.ADMIN, AccountRoles.MODERATOR }) ||
                    message.CreatedBy.Id == int.Parse(requestFromId))
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


        [Authorize(Roles = "ADMIN,MODERATOR")]
        [HttpPatch]
        public ActionResult<Message> PatchMessage(Message message)
        {
            var dbMessage = _messagesRepository.GetByID(message.Id);
            if (dbMessage == null)
            {
                return new ObjectResult(null) { StatusCode = StatusCodes.Status404NotFound };
            }

            var result = _messagesRepository.Patch(message);

            return new ObjectResult(result);
        }
    }
}