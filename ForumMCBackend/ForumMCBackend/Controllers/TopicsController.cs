using ForumMCBackend.Models;
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
        private readonly IAccountsRepository _accountsRepository;
        private readonly ITopicsRepository _topicsRepository;
        private readonly IMessagesRepository _messagesRepository;

        public TopicsController(ICategoriesRepository categoriesRepository, IAccountsRepository accountsRepository, ITopicsRepository topicsRepository, IMessagesRepository messagesRepository)
        {
            _categoriesRepository = categoriesRepository;
            _accountsRepository = accountsRepository;
            _topicsRepository = topicsRepository;
            _messagesRepository = messagesRepository;
        }

        [Authorize]
        [HttpPost]
        public ActionResult<Topic> Post(Topic topic)
        {
            if (topic.Title == null || topic.Category == null)
            {
                return new ObjectResult(null) { StatusCode = StatusCodes.Status400BadRequest };
            }

            var category = _categoriesRepository.GetByID(topic.Category.Id);
            if (category == null)
            {
                return new ObjectResult(null) { StatusCode = StatusCodes.Status400BadRequest };
            }
            else
            {
                topic.Category = category;
            }

            var requestFromId = HttpContext.User.Identity?.Name ?? "0";
            var requestFrom = _accountsRepository.GetByID(int.Parse(requestFromId));
            topic.CreatedBy = requestFrom;

            var result = _topicsRepository.Add(topic);
            result.CreatedBy.Password = null;
            return new ObjectResult(result) { StatusCode = StatusCodes.Status201Created };
        }

        [HttpGet]
        public ActionResult<List<Topic>> GetLatestTenTopics()
        {
            var listOfTopics = _topicsRepository.GetLatestTenTopics();

            var requestFromId = HttpContext.User.Identity?.Name ?? "0";
            var requestFrom = _accountsRepository.GetByID(int.Parse(requestFromId)) ?? new Account();

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
        public ActionResult<Topic> PatchTopic(Topic topic)
        {
            var dbTopic = _topicsRepository.GetByID(topic.Id);
            if (dbTopic == null)
            {
                return new ObjectResult(null) { StatusCode = StatusCodes.Status404NotFound };
            }

            var requestFromId = HttpContext.User.Identity?.Name ?? "0";
            var requestFrom = _accountsRepository.GetByID(int.Parse(requestFromId)) ?? new Account();
            if (requestFrom.Role != AccountRoles.ADMIN && requestFrom.Role != AccountRoles.MODERATOR)
            {
                return new ObjectResult(null) { StatusCode = StatusCodes.Status401Unauthorized };
            }

            var result = _topicsRepository.Patch(topic);

            return new ObjectResult(result);
        }

        [HttpGet("{topicId}/messages")]
        public ActionResult<List<Message>> GetTopicMessages(int topicId)
        {
            var messagesOfTopic = _messagesRepository.GetByTopicID(topicId);

            var requestFromId = HttpContext.User.Identity?.Name ?? "0";
            var requestFrom = _accountsRepository.GetByID(int.Parse(requestFromId)) ?? new Account();

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