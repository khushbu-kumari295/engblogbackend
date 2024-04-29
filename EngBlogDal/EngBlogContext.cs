using EngBlogJob.Models;
using Microsoft.EntityFrameworkCore;

namespace EngBlogJob
{
    public class EngBlogContext : DbContext, IEngBlogContext
    {
        public EngBlogContext()
        {
        }

        public EngBlogContext(DbContextOptions<EngBlogContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Article> Articles { get; set; }

        public Task<int> SaveChangesAsync(List<Article> articles)
        {
            var existingUniqueIds = Articles.Select(a => a.UniqueId).ToList();
            var existingArticles = articles.Where(a => existingUniqueIds.Contains(a.UniqueId)).ToList();
            var newArticles = articles.Where(a => !existingUniqueIds.Contains(a.UniqueId)).ToList();

            if (newArticles.Count != 0)
            {
                Articles.AddRange(newArticles);
            }

            return SaveChangesAsync();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Article>()
                .ToContainer("Articles")
                .HasPartitionKey(a => a.Id);
        }
    }
}
