using ForumMCBackend.Models;
using ForumMCBackend.Models.DTOs;
using ForumMCBackend.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace ForumMCBackend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoriesRepository _categoriesRepository;
        private readonly ITopicsRepository _topicsRepository;

        public CategoriesController(ICategoriesRepository categoriesRepository, ITopicsRepository topicsRepository)
        {
            _categoriesRepository = categoriesRepository;
            _topicsRepository = topicsRepository;
        }

        [HttpGet]
        public ActionResult<List<Category>> Get()
        {
            var categories = _categoriesRepository.GetAll();
            return categories;
        }

        [HttpGet("{id}/topics")]
        public ActionResult<List<TopicDTO>> GetTopics(int id)
        {
            var listOfTopics = _topicsRepository.GetByCategoryID(id).Select((topic) =>
                new TopicDTO {
                    Id = topic.Id,
                    Title = topic.Title,
                    CreatedBy = new AccountDTO
                    {
                        Id = topic.CreatedBy.Id,
                        UserName = topic.CreatedBy.UserName,
                        Role = topic.CreatedBy.Role
                    },
                    Category = topic.Category,
                    IsHidden = topic.IsHidden
                }
            );
            var topics = new List<TopicDTO>();

            foreach (var dbTopic in listOfTopics)
            {
                if (dbTopic.IsHidden &&
                    Account.IsInRoles(HttpContext.User, new List<AccountRoles>{ AccountRoles.ADMIN, AccountRoles.MODERATOR}))
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