using EngBlogJob;
using EngBlogJob.Models;
using Microsoft.EntityFrameworkCore;

namespace EngBlogApi.Manager
{
    public class ArticlesManager : IArticlesManager
    {
        private Lazy<IEnumerable<Article>> _lazyArticles;
        private readonly ILogger<ArticlesManager> _logger;
        private readonly IEngBlogContext _dbContext;
        private readonly IServiceScopeFactory _scopeFactory;

        public ArticlesManager(ILogger<ArticlesManager> logger, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
            _lazyArticles = new Lazy<IEnumerable<Article>>();
        }

        public IEnumerable<Article> GetArticles()
        {
            return _lazyArticles.Value;
        }

        public IEnumerable<Article> GetArticles(string owner)
        {
            if (owner == null || owner.Trim() == string.Empty)
            {
                return new List<Article>();
            }
            return GetArticles().Where(a => owner.Equals(a.Owner, StringComparison.OrdinalIgnoreCase));
        }

        public async Task RefreshData()
        {
            _logger.LogInformation("Refreshing the data");
            var newData = await RetrieveData();
            Interlocked.Exchange(ref _lazyArticles, new Lazy<IEnumerable<Article>>(() => newData));
            _logger.LogInformation("Data refreshed!!");
        }

        private async Task<IEnumerable<Article>> RetrieveData()
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<IEngBlogContext>();
                return db.Articles.ToList();
            }
        }
    }
}
