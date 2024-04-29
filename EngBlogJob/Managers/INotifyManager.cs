using EngBlogJob.Models;

namespace EngBlogJob.Managers
{
    public class NotifyPayload
    {
        public int Count { get; set; }
    }

    public interface INotifyManager
    {
        NotifyPayload NotifyNewArticles(List<Article> articles);
    }
}
