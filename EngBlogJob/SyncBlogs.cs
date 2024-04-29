using EngBlogJob.Managers;
using EngBlogJob.Models;
using EngBlogJob.Parser;
using Microsoft.Extensions.Logging;

namespace EngBlogJob
{
    public class SyncBlogs
    {
        private readonly ILogger _logger;
        private readonly IParserFactory _parserFactory;
        private readonly IEngBlogContext _blogDbContext;
        private readonly INotifyManager _notifyManager;

        public SyncBlogs(ILoggerFactory loggerFactory, IParserFactory parserFactory, IEngBlogContext engDb, INotifyManager notifyManager)
        {
            _logger = loggerFactory.CreateLogger<SyncBlogs>();
            _parserFactory = parserFactory;
            _blogDbContext = engDb;
            _notifyManager = notifyManager;
        }

        public async Task Run()
        {
            _logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            await SyncBlogsCore();
        }

        private async Task SyncBlogsCore()
        {
            var articles = new List<Article>();
            articles.AddRange((await _parserFactory.GetBlogParser("github")?.GetArticles()) ?? new List<Article>());
            articles.AddRange((await _parserFactory.GetBlogParser("meta")?.GetArticles()) ?? new List<Article>());
            articles.AddRange((await _parserFactory.GetBlogParser("heroku")?.GetArticles()) ?? new List<Article>());
            articles.AddRange((await _parserFactory.GetBlogParser("dotnet")?.GetArticles()) ?? new List<Article>());
            articles.AddRange((await _parserFactory.GetBlogParser("google")?.GetArticles()) ?? new List<Article>());
            _notifyManager.NotifyNewArticles(articles);
            await _blogDbContext.SaveChangesAsync(articles);
        }
    }
}
