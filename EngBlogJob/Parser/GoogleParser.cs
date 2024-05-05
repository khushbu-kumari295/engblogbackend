using EngBlogJob.Clients;
using EngBlogJob.Models;
using Microsoft.Extensions.Logging;

namespace EngBlogJob.Parser
{
    public class GoogleParser : IBlogParser
    {
        private readonly ILogger _logger;
        private readonly IWebClient _webClient;
        private static string _baseUrl = "https://developers.googleblog.com/";

        public GoogleParser(ILoggerFactory loggerFactory, IWebClient webClient)
        {
            _logger = loggerFactory.CreateLogger<GoogleParser>();
            _webClient = webClient;
        }

        public async Task<List<Article>> GetArticles()
        {
            var articles = new List<Article>();
            for (int page = 1; page <= 4; page++)
            {
                articles.AddRange(await GetPageArticle($"{_baseUrl}/en/search/?technology_categories=Mobile%2CWeb%2CAI%2CCloud&page={page}"));
                await Task.Delay(1000);
            }
            return articles;
        }

        private async Task<List<Article>> GetPageArticle(string url)
        {
            var doc = _webClient.GetUrlContent(url);
            var articleList = new List<Article>();
            var articles = doc.DocumentNode.SelectNodes("//li[@class='search-result']");

            if (articles != null)
            {
                foreach (var articleNode in articles)
                {
                    string dateAndCategory = articleNode.SelectSingleNode(".//p[@class='search-result__eyebrow']").InnerText;
                    var splitted = dateAndCategory.Split('/');
                    var date = splitted[0].Trim();
                    var category = splitted[1].Trim();

                    string title = articleNode.SelectSingleNode(".//h3/a").InnerText;
                    string summary = articleNode.SelectSingleNode(".//p[@class='search-result__summary']").InnerText;
                    string linkAddr = articleNode.SelectSingleNode(".//h3/a").GetAttributeValue("href", "");
                    if (!linkAddr.StartsWith("https://"))
                    {
                        linkAddr = $"{_baseUrl}{linkAddr}";
                    }
                    DateTimeOffset? timeCreated = date != null ? DateTimeOffset.Parse(date) : null;

                    string imageUrl = articleNode.SelectSingleNode(".//img").GetAttributeValue("src", "");

                    if (title != null && linkAddr != null )
                    {
                        var newArticle = new Article
                        {
                            Id = Guid.NewGuid().ToString(),
                            UniqueId = linkAddr,
                            Name = title,
                            Description = summary,
                            ImageSrc = imageUrl,
                            Owner = "Google",
                            Link = linkAddr,
                            UploadedTimestamp = timeCreated,
                            Author = null,
                            Category = category,
                            CrawledTimestamp = DateTimeOffset.Now
                        };
                        articleList.Add(newArticle);
                        _logger.LogInformation("Found article: {}", newArticle);
                    }
                }

            }
            return articleList;
        }
    }
}
