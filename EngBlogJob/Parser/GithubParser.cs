using EngBlogJob.Clients;
using EngBlogJob.Models;
using Microsoft.Extensions.Logging;

namespace EngBlogJob.Parser
{
    public class GithubParser: IBlogParser
    {
        private readonly ILogger _logger;
        private readonly IWebClient _webClient;
        private static string _baseUrl = "https://github.blog";

        public GithubParser(ILoggerFactory loggerFactory, IWebClient webClient)
        {
            _logger = loggerFactory.CreateLogger<GithubParser>();
            _webClient = webClient;
        }

        public async Task<List<Article>> GetArticles()
        {
            var articles = new List<Article>();
            for (int page = 1; page <= 4; page++)
            {
                articles.AddRange(FetchData($"{_baseUrl}/category/engineering/page/{page}/"));
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
                    var linkToArticle = article.SelectSingleNode(".//a[contains(@class, 'Link--primary')]");
                    var link = linkToArticle?.GetAttributeValue("href", "");
                    var title = linkToArticle?.InnerText.Trim();
                    var id = article.GetAttributeValue("id", "");
                    var description = article.SelectSingleNode(".//p")?.InnerText.Trim();

                    var imageUrl = article.SelectSingleNode(".//img")?.GetAttributeValue("srcset", "");
                    var time = DateTimeOffset.Parse(article.SelectSingleNode(".//time").InnerText);
                    var author = article.SelectSingleNode(".//span[contains(@class, 'authors-wrap')]")?.InnerText.Trim();

                    if (title != null && link != null && id != null)
                    {
                        var newArticle = new Article
                        {
                            Id = Guid.NewGuid().ToString(),
                            UniqueId = id,
                            Name = title,
                            Description = description,
                            ImageSrc = imageUrl,
                            Owner = "Github",
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
