using EngBlogJob.Clients;
using EngBlogJob.Models;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using Microsoft.Playwright;

namespace EngBlogJob.Parser
{
    public class GoogleParser : IBlogParser
    {
        private readonly ILogger _logger;
        private readonly IWebClient _webClient;
        private static string _baseUrl = "https://developers.googleblog.com/";

        public GoogleParser(ILoggerFactory loggerFactory, IWebClient webClient)
        {
            _logger = loggerFactory.CreateLogger<HerokuParser>();
            _webClient = webClient;
        }

        public async Task<List<Article>> GetArticles()
        {
            var articles = new List<Article>();
            await _webClient.GetUrlContentAfterAction(_baseUrl, articles, ParsePage);
            return articles;
        }

        private async Task ParsePage(IPage page, List<Article> articles)
        {
            for(int i=1; i <= 4; i++)
            {
                var pageContent = await page.ContentAsync();
                var doc = new HtmlDocument();
                doc.LoadHtml(pageContent);
                articles.AddRange( await GetPageArticle(doc));                

                await page.GetByRole(AriaRole.Link, new() { NameString = "Previous posts" }).ClickAsync();
            }
        }

        private async Task<List<Article>> GetPageArticle(HtmlDocument doc)
        {
            var articles = new List<Article>();
            var docArticles = doc.DocumentNode.SelectNodes("//div[contains(@class, 'dgc-card')]");

            if (docArticles != null)
            {
                foreach(var docArticle in docArticles)
                {
                    var linkElement = docArticle.SelectSingleNode(".//a[contains(@class, 'dgc-card__href')]");
                    if (linkElement == null)
                    {
                        continue;
                    }
                    var link = linkElement.GetAttributeValue("href", null);
                    var id = link;

                    var imageElement = docArticle.SelectSingleNode(".//img[contains(@class, 'dgc-card__image')]");
                    var imageUrl = imageElement.GetAttributeValue("src", null);

                    var titleElement = docArticle.SelectSingleNode(".//div[contains(@class, 'dgc-card__title')]");
                    var title = titleElement?.InnerText?.Trim();

                    var descriptionElement = docArticle.SelectSingleNode(".//div[contains(@class, 'dgc-card__description')]");
                    var description = descriptionElement?.InnerText?.Trim();

                    var timeElement = docArticle.SelectSingleNode(".//div[contains(@class, 'dgc-card__info')]");
                    var timeStr = timeElement?.InnerText?.Trim();
                    DateTimeOffset? time = timeStr != null ? DateTimeOffset.Parse(timeStr) : null;

                    if (title != null && link != null && id != null)
                    {
                        var newArticle = new Article
                        {
                            Id = Guid.NewGuid().ToString(),
                            UniqueId = id,
                            Name = title,
                            Description = description,
                            ImageSrc = imageUrl,
                            Owner = "Google",
                            Link = link,
                            UploadedTimestamp = time,
                            Author = null,
                            CrawledTimestamp = DateTimeOffset.Now
                        };
                        articles.Add(newArticle);
                        _logger.LogInformation("Found article: {}", newArticle);
                    }
                }
            }
            return articles;
        }
    }
}
