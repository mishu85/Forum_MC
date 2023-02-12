using ApplicationCore.Entities;
using ApplicationCore.Entities.DTOs;
using Ardalis.GuardClauses;
using AutoMapper;
using ForumMCBackend.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ForumMCBackend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TopicsController : ControllerBase
    {
        private readonly ICategoriesRepository _categoriesRepository;
        private readonly ITopicsRepository _topicsRepository;
        private readonly IMessagesRepository _messagesRepository;
        private readonly IMapper _mapper;

        public TopicsController(ICategoriesRepository categoriesRepository, ITopicsRepository topicsRepository, IMessagesRepository messagesRepository, IMapper mapper)
        {
            _categoriesRepository = categoriesRepository;
            _topicsRepository = topicsRepository;
            _messagesRepository = messagesRepository;
            _mapper = mapper;
        }

        [Authorize]
        [HttpPost]
        public ActionResult<TopicDTO> Post(Topic topic)
        {
            var category = _categoriesRepository.GetByID(topic.CategoryId);
            Guard.Against.Null(category, nameof(category));

            var requestFromId = HttpContext.User.Identity?.Name ?? "0";
            topic.UpdateCreatedBy(int.Parse(requestFromId));

            var result = _topicsRepository.Add(topic);
            return new ObjectResult(_mapper.Map<TopicDTO>(result)) { StatusCode = StatusCodes.Status201Created };
        }

        [HttpGet]
        public ActionResult<List<TopicDTO>> GetLatestTenTopics()
        {
            var listOfTopics = _topicsRepository.GetLatestTenTopics();
            var topics = new List<TopicDTO>();

            foreach (var dbTopic in listOfTopics)
            {
                if (dbTopic.IsHidden &&
                    Account.IsInRoles(HttpContext.User, new List<AccountRoles> { AccountRoles.ADMIN, AccountRoles.MODERATOR }))
                {
                    topics.Add(_mapper.Map<TopicDTO>(dbTopic));
                }
                else if (!dbTopic.IsHidden)
                {
                    topics.Add(_mapper.Map<TopicDTO>(dbTopic));
                }
            }

            return new ObjectResult(topics);
        }

        [Authorize(Roles = "ADMIN,MODERATOR")]
        [HttpPatch]
        public ActionResult<TopicDTO> PatchTopic(Topic topic)
        {
            var dbTopic = _topicsRepository.GetByID(topic.Id);
            Guard.Against.Null(dbTopic, nameof(dbTopic));

            var result = _topicsRepository.Patch(topic);

            return new ObjectResult(_mapper.Map<TopicDTO>(result));
        }

        [HttpGet("{topicId}/messages")]
        public ActionResult<List<MessageDTO>> GetTopicMessages(int topicId)
        {
            var messagesOfTopic = _messagesRepository.GetByTopicID(topicId);
            var requestFromId = HttpContext.User.Identity?.Name ?? "0";

            var messages = new List<MessageDTO>();
            var now = DateTime.UtcNow;
            foreach (var message in messagesOfTopic)
            {
                if (Account.IsInRoles(HttpContext.User, new List<AccountRoles> { AccountRoles.ADMIN, AccountRoles.MODERATOR }) ||
                    message.CreatedById == int.Parse(requestFromId))
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
    }
}