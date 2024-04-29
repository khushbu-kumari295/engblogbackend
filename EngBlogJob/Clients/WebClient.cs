using EngBlogJob.Models;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using Microsoft.Playwright;
using System.Net.NetworkInformation;

namespace EngBlogJob.Clients
{
    public class WebClient: IWebClient
    {
        private readonly ILogger _logger;

        public WebClient(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<WebClient>();
        }

        public HtmlDocument GetUrlContent(string url)
        {
            var web = new HtmlWeb();
            _logger.LogInformation("Calling endpoint {url} to get data", url);
            try
            {
                return web.Load(url);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Issue calling {}", url);
                return null;
            }
        }

        public async Task<HtmlDocument> GetUrlContentAfterAction(string url, Func<IPage, Task> action)
        {
            using var playwright = await Playwright.CreateAsync();
            await using var browser = await playwright.Chromium.LaunchAsync();
            var page = await browser.NewPageAsync();
            await page.GotoAsync(url);
            await action(page);
            var pageContent = await page.ContentAsync();
            var doc = new HtmlDocument();
            doc.LoadHtml(pageContent);
            return doc;
        }

        public async Task<HtmlDocument> GetUrlContentAfterAction(string url, List<Article> articles, Func<IPage, List<Article>, Task> action)
        {
            using var playwright = await Playwright.CreateAsync();
            await using var browser = await playwright.Chromium.LaunchAsync();
            var page = await browser.NewPageAsync();
            await page.GotoAsync(url);
            await action(page, articles);
            var pageContent = await page.ContentAsync();
            var doc = new HtmlDocument();
            doc.LoadHtml(pageContent);
            return doc;
        }
    }
}
