using EngBlogJob.Clients;
using EngBlogJob.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Playwright;

namespace EngBlogJob.Parser
{
    public class MetaParser : IBlogParser
    {
        private readonly ILogger _logger;
        private readonly IWebClient _webClient;
        private static string _baseUrl = "https://engineering.fb.com/";

        public MetaParser(ILoggerFactory loggerFactory, IWebClient webClient)
        {
            _logger = loggerFactory.CreateLogger<MetaParser>();
            _webClient = webClient;
        }

        public async Task<List<Article>> GetArticles()
        {
            var doc = await _webClient.GetUrlContentAfterAction(_baseUrl, async (IPage page) =>
            {
                await page.GetByRole(AriaRole.Button, new() { NameString = "Load More" }).ClickAsync();
                await page.GetByRole(AriaRole.Button, new() { NameString = "Load More" }).ClickAsync();
                await page.GetByRole(AriaRole.Button, new() { NameString = "Load More" }).ClickAsync();
                await page.GetByRole(AriaRole.Button, new() { NameString = "Load More" }).ClickAsync();
                await page.GetByRole(AriaRole.Button, new() { NameString = "Load More" }).ClickAsync();
                await page.GetByRole(AriaRole.Button, new() { NameString = "Load More" }).ClickAsync();
            });

            var articles = new List<Article>();
            if (doc == null)
            {
                return articles;
            }
            var articleNodes = doc.DocumentNode.SelectNodes($"//article");
            if (articleNodes != null)
            {
                foreach ( var node in articleNodes )
                {
                    var entryTitle = node.SelectSingleNode(".//div[contains(@class, 'entry-title')]");
                    var linkToArticle = node.SelectSingleNode(".//a[contains(@rel, 'bookmark')]");
                    var link = linkToArticle?.GetAttributeValue("href", "");
                    var title = entryTitle.InnerText?.Trim();
                    var uniqueId = node.GetAttributeValue("id", "");
                    var description = "";
                    var imageUrl = node.SelectSingleNode(".//img")?.GetAttributeValue("src", "");
                    var time = node.SelectSingleNode(".//time")?.InnerText != null ? 
                        DateTimeOffset.Parse(node.SelectSingleNode(".//time")?.InnerText) : 
                        DateTimeOffset.Now;
                    var author = node.SelectSingleNode(".//span[contains(@class, 'cat-links')]")?.InnerText.Trim();

                    if (title != null && link != null && uniqueId != null)
                    {
                        var newArticle = new Article
                        {
                            Id = Guid.NewGuid().ToString(),
                            UniqueId = uniqueId,
                            Name = title,
                            Description = description,
                            ImageSrc = imageUrl,
                            Owner = "Meta",
                            Link = link,
                            UploadedTimestamp = time,
                            Author = author,
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
