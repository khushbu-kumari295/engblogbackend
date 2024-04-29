using EngBlogJob.Clients;
using EngBlogJob.Models;
using Microsoft.Extensions.Logging;

namespace EngBlogJob.Parser
{
    public class HerokuParser : IBlogParser
    {
        private readonly ILogger _logger;
        private readonly IWebClient _webClient;
        private static string _baseUrl = "https://blog.heroku.com/engineering";

        public HerokuParser(ILoggerFactory loggerFactory, IWebClient webClient)
        {
            _logger = loggerFactory.CreateLogger<HerokuParser>();
            _webClient = webClient;
        }

        public async Task<List<Article>> GetArticles()
        {
            var articles = new List<Article>();
            for (int page = 1; page <= 4; page++)
            {
                articles.AddRange(FetchData($"{_baseUrl}?page={page}"));
                await Task.Delay(1000); // Be kind to the website
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
                    var metaMain = article.SelectSingleNode(".//meta[contains(@itemprop, 'mainEntityOfPage')]");
                    var link = metaMain.GetAttributeValue("content", null);

                    var titleH1 = article.SelectSingleNode(".//h1[contains(@itemprop, 'name')]");
                    var title = titleH1?.InnerText?.Trim();

                    var id = article.GetAttributeValue("itemid", null);
                    var postSnippet = article.SelectSingleNode(".//div[contains(@class, 'post-snippet')]/p");
                    var description = postSnippet?.InnerText?.Trim();                   

                    string imageUrl = null;
                    var time = DateTimeOffset.Parse(article.SelectSingleNode(".//time").InnerText);
                    var author = article.SelectSingleNode(".//span[contains(@class, 'author')]")?.InnerText.Trim();

                    if (title != null && link != null && id != null)
                    {
                        var newArticle = new Article
                        {
                            Id = Guid.NewGuid().ToString(),
                            UniqueId = id,
                            Name = title,
                            Description = description,
                            ImageSrc = imageUrl,
                            Owner = "Heroku",
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
