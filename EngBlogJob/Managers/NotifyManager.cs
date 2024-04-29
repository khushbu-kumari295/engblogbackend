using EngBlogJob.Models;
using Microsoft.Extensions.Logging;

namespace EngBlogJob.Managers
{
    public class NotifyManager : INotifyManager
    {
        private readonly IEngBlogContext _blogDbContext;
        private readonly ILogger<NotifyManager> _logger;

        public NotifyManager(IEngBlogContext blogDbContext, ILoggerFactory loggerFactory)
        {
            _blogDbContext = blogDbContext;
            _logger = loggerFactory.CreateLogger<NotifyManager>();
        }

        public NotifyPayload NotifyNewArticles(List<Article> articles)
        {
            var existing = _blogDbContext.Articles.Select(a => a.UniqueId).ToList();
            var newArticles = articles.Where(a => !existing.Contains(a.UniqueId));
            _logger.LogInformation("Found {count} new articles", newArticles.Count());
            return new NotifyPayload { Count = newArticles.Count() };
        }
    }
}
