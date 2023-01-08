using ForumMCBackend.Models;
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
        public ActionResult<List<Topic>> GetTopics(int id)
        {
            var listOfTopics = _topicsRepository.GetByCategoryID(id);
            var topics = new List<Topic>();

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