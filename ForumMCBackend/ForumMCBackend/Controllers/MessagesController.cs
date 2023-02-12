using AutoMapper;
using ApplicationCore.Entities;
using ApplicationCore.Entities.DTOs;
using ForumMCBackend.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ardalis.GuardClauses;

namespace ForumMCBackend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MessagesController : ControllerBase
    {
        private readonly IMessagesRepository _messagesRepository;
        private readonly ITopicsRepository _topicsRepository;
        private readonly IMapper _mapper;

        public MessagesController(IMessagesRepository messagesRepository, ITopicsRepository topicsRepository, IMapper mapper)
        {
            _messagesRepository = messagesRepository;
            _topicsRepository = topicsRepository;
            _mapper = mapper;
        }

        [Authorize]
        [HttpPost]
        public ActionResult<MessageDTO> Post(Message message)
        {
            var topic = _topicsRepository.GetByID(message.TopicId);
            Guard.Against.Null(topic, nameof(topic));

            if (message.InReplyToId != null)
            {
                var inReplyTo = _messagesRepository.GetByID(message.InReplyToId ?? 0);
                Guard.Against.Null(inReplyTo, nameof(inReplyTo));
                Guard.Against.NoRepliesToReplies(inReplyTo);
            }

            var requestFromId = HttpContext.User.Identity?.Name ?? "0";
            message.UpdateCreatedBy(int.Parse(requestFromId));

            var result = _messagesRepository.Add(message);
            return new ObjectResult(_mapper.Map<MessageDTO>(result)) { StatusCode = StatusCodes.Status201Created };
        }

        [HttpGet("{messageId}/replies")]
        public ActionResult<List<MessageDTO>> GetMessageReplies(int messageId)
        {
            var repliesToMessage = _messagesRepository.GetReplies(messageId);
            var requestFromId = HttpContext.User.Identity?.Name ?? "0";

            var messages = new List<MessageDTO>();
            var now = DateTime.UtcNow;
            foreach (var message in repliesToMessage)
            {
                if (Account.IsInRoles(HttpContext.User, new List<AccountRoles> { AccountRoles.ADMIN, AccountRoles.MODERATOR }) ||
                    message.CreatedById== int.Parse(requestFromId))
                {
                    messages.Add(_mapper.Map<MessageDTO>(message));
                }
                else
                {
                    if (message.IsHidden)
                    {
                        message.UpdateBodyText("");
                    }
                    if (now > message.GoesLive)
                    {
                        messages.Add(_mapper.Map<MessageDTO>(message));
                    }
                }
            }

            return messages;
        }


        [Authorize(Roles = "ADMIN,MODERATOR")]
        [HttpPatch]
        public ActionResult<MessageDTO> PatchMessage(Message message)
        {
            var dbMessage = _messagesRepository.GetByID(message.Id);
            Guard.Against.Null(dbMessage, nameof(dbMessage));

            var result = _messagesRepository.Patch(message);

            return new ObjectResult(result);
        }
    }
}