using EngBlogJob.Models;
using Microsoft.EntityFrameworkCore;

namespace EngBlogJob
{
    public interface IEngBlogContext
    {
        DbSet<Article> Articles { get; set; }

        Task<int> SaveChangesAsync(List<Article> articles);

        int SaveChanges();

        int SaveChanges(bool acceptAllChangesOnSuccess);

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default);
    }
}