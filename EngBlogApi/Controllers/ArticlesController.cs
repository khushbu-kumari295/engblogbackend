using EngBlogApi.Manager;
using EngBlogJob.Models;
using Microsoft.AspNetCore.Mvc;

namespace EngBlogApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ArticlesController : ControllerBase
    {   
        private readonly ILogger<ArticlesController> _logger;
        private readonly IArticlesManager _articlesManager;

        public ArticlesController(ILogger<ArticlesController> logger, IArticlesManager articlesManager)
        {
            _logger = logger;
            _articlesManager = articlesManager;
        }

        [HttpGet("")]
        public IEnumerable<Article> GetArticles()
        {
            return _articlesManager.GetArticles();
        }
    }
}
