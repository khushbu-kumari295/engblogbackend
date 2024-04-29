using EngBlogJob.Clients;
using Microsoft.Extensions.Logging;

namespace EngBlogJob.Parser
{
    internal class ParserFactory: IParserFactory
    {
        private readonly ILogger _logger;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IWebClient _webClient;

        public ParserFactory(ILoggerFactory loggerFactory, IWebClient webClient)
        {
            _logger = loggerFactory.CreateLogger<ParserFactory>();
            _loggerFactory = loggerFactory;
            _webClient = webClient;
        }

        public IBlogParser? GetBlogParser(string name)
        {
            return name.ToLower() switch
            {
                "github" => new GithubParser(_loggerFactory, _webClient),
                "meta" => new MetaParser(_loggerFactory, _webClient),
                "heroku" => new HerokuParser(_loggerFactory, _webClient),
                "dotnet" => new DotnetParser(_loggerFactory, _webClient),
                "google" => new GoogleParser(_loggerFactory, _webClient),
                _ => null
            };
        }
    }
}
