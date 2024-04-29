namespace EngBlogJob.Parser
{
    public interface IParserFactory
    {
        IBlogParser? GetBlogParser(string name);
    }
}
