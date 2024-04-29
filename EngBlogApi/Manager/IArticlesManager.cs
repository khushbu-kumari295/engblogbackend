using EngBlogJob.Models;

namespace EngBlogApi.Manager
{
    public interface IArticlesManager
    {
        IEnumerable<Article> GetArticles();
        IEnumerable<Article> GetArticles(string owner);
        Task RefreshData();
    }
}