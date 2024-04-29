using EngBlogJob.Clients;
using EngBlogJob.Models;
using Microsoft.Extensions.Logging;

namespace EngBlogJob.Parser
{
    public class DotnetParser : IBlogParser
    {
        private readonly ILogger _logger;
        private readonly IWebClient _webClient;
        private static string _baseUrl = "https://devblogs.microsoft.com/dotnet";
        private static string _name = "Microsoft";

        public DotnetParser(ILoggerFactory loggerFactory, IWebClient webClient)
        {
            _logger = loggerFactory.CreateLogger<GithubParser>();
            _webClient = webClient;
        }

        public async Task<List<Article>> GetArticles()
        {
            var articles = new List<Article>();
            for (int page = 1; page <= 4; page++)
            {
                articles.AddRange(FetchData($"{_baseUrl}/page/{page}/"));
                await Task.Delay(1000);
            }
            return articles;
        }

        private List<Article> FetchData(string url)
        {
            var doc = _webClient.GetUrlContent(url);

            var articleLinks = new List<Article>();
            if (doc == null)
            {
                return articleLinks;
            }

            var articles = doc.DocumentNode.SelectNodes($"//article");
            if (articles != null)
            {
                foreach (var article in articles)
                {
                    var id = article.GetAttributeValue("id", null);

                    var titleElement = article.SelectSingleNode(".//h2[contains(@class, 'entry-title')]/a");
                    var title = titleElement?.InnerText?.Trim();
                    var link = titleElement.GetAttributeValue("href", null);

                    var imageElement = article.SelectSingleNode(".//img[contains(@class, 'lp-default-image-blog')]");
                    var image = imageElement?.GetAttributeValue("data-src", null);

                    var timeText = article.SelectSingleNode(".//span[contains(@class, 'entry-post-date')]")?.InnerText?.Trim();
                    DateTimeOffset? time = timeText != null ? DateTimeOffset.Parse(timeText) : null;

                    var author = article.SelectSingleNode(".//span[contains(@class, 'entry-author-link')]")?.InnerText.Trim();

                    if (title != null && link != null && id != null)
                    {
                        var newArticle = new Article
                        {
                            Id = Guid.NewGuid().ToString(),
                            UniqueId = id,
                            Name = title,
                            Description = null,
                            ImageSrc = image,
                            Owner = _name,
                            Link = link,
                            UploadedTimestamp = time,
                            Author = author,
                            CrawledTimestamp = DateTimeOffset.Now
                        };
                        articleLinks.Add(newArticle);
                        _logger.LogInformation("Found article: {}", newArticle);
                    }
                }
            }

            return articleLinks;
        }
    }
}
