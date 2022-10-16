using ForumMCBackend.Db;
using Microsoft.AspNetCore.Mvc;


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
    }
}