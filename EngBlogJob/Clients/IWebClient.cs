using EngBlogJob.Models;
using HtmlAgilityPack;
using Microsoft.Playwright;

namespace EngBlogJob.Clients
{
    public interface IWebClient
    {
        HtmlDocument GetUrlContent(string url);
        Task<HtmlDocument> GetUrlContentAfterAction(string url, Func<IPage, Task> action);

        Task<HtmlDocument> GetUrlContentAfterAction(string url, List<Article> articles, Func<IPage, List<Article>, Task> action);
    }
}
