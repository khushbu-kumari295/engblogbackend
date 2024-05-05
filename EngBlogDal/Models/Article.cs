using System.Reflection;
using System.Text;

namespace EngBlogJob.Models
{
    public class Article
    {
        public string Id { get; set; }

        public string Name { get; set; } = null!;

        public string Link { get; set; } = null!;

        public string? Owner { get; set; }

        public string UniqueId { get; set; } = null!;

        public string? ImageSrc { get; set; }

        public string? Description { get; set; }

        public string? Author { get; set; }

        public string? Category { get; set; }

        public DateTimeOffset? UploadedTimestamp { get; set; }

        public DateTimeOffset? CrawledTimestamp { get; set; }

        public override string ToString()
        {
            Type type = this.GetType();
            PropertyInfo[] properties = type.GetProperties();
            var result = new StringBuilder();

            foreach (PropertyInfo property in properties)
            {
                var value = property.GetValue(this);
                result.Append($"{property.Name}: {value}\n");
            }

            return result.ToString();
        }
    }

}
