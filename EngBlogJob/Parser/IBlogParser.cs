using EngBlogJob.Models;

namespace EngBlogJob.Parser
{
    public interface IBlogParser
    {
        public Task<List<Article>> GetArticles();
    }
}
