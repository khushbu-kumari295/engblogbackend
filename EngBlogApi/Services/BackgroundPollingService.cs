using EngBlogApi.Manager;

namespace EngBlogApi.Services
{
    public class BackgroundPollingService : BackgroundService
    {
        private readonly ILogger<BackgroundPollingService> _logger;
        private readonly IArticlesManager _articlesManager;
        private readonly int _pollingIntervalInMins;

        public BackgroundPollingService(ILogger<BackgroundPollingService> logger, IArticlesManager articlesManager, int pollingIntervalInMins)
        {
            _logger = logger;
            _articlesManager = articlesManager;
            _pollingIntervalInMins = pollingIntervalInMins;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation($"Task Started at: {DateTime.Now}");
                await _articlesManager.RefreshData();
                _logger.LogInformation($"Task Completed at: {DateTime.Now}. Now Waiting for {_pollingIntervalInMins} mins");
                await Task.Delay(TimeSpan.FromMinutes(_pollingIntervalInMins), stoppingToken);
            }
        }
    }
}
