using ApplicationCore.Entities;
using ApplicationCore.Entities.DTOs;
using AutoMapper;
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
        private readonly IMapper _mapper;

        public CategoriesController(ICategoriesRepository categoriesRepository, ITopicsRepository topicsRepository, IMapper mapper)
        {
            _categoriesRepository = categoriesRepository;
            _topicsRepository = topicsRepository;
            _mapper = mapper;
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
                _mapper.Map<TopicDTO>(topic)
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