using EngBlogJob;
using EngBlogJob.Clients;
using EngBlogJob.Managers;
using EngBlogJob.Parser;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Playwright;

#region Install Playwright Browser
using (var playwright = await Playwright.CreateAsync())
{
    try
    {
        await using var browser = await playwright.Chromium.LaunchAsync();
    }
    catch
    {
        var exitCode = Microsoft.Playwright.Program.Main(new[] { "install", "--with-deps", "chromium" });
        if (exitCode != 0)
        {
            throw new Exception($"Playwright exited with code {exitCode}");
        }
    }
}    
#endregion

var hostBuilder = Host.CreateDefaultBuilder(args);

hostBuilder.ConfigureServices((context, services) =>
{
    var socketsHttpHandler = new SocketsHttpHandler();
    socketsHttpHandler.PooledConnectionLifetime = TimeSpan.FromMinutes(5);
    services.AddSingleton(socketsHttpHandler);
    var cosmosDbKey = Environment.GetEnvironmentVariable("COSMOS_DB_KEY");
    if (cosmosDbKey == null)
    {
        throw new Exception("Cosmos DB Key is not provided");
    }

    services.AddDbContext<IEngBlogContext, EngBlogContext>(options => options.UseCosmos(
        "https://kksideprojects.documents.azure.com:443/", 
        cosmosDbKey, 
        "EngBlogDb", (o) =>
        {
            o.HttpClientFactory(() => new HttpClient(socketsHttpHandler, disposeHandler: false));
        } ));

    services.AddSingleton<IWebClient, WebClient>();
    services.AddSingleton<INotifyManager, NotifyManager>();
    services.AddSingleton<IParserFactory, ParserFactory>();
    services.AddSingleton<SyncBlogs>();
});

var host = hostBuilder.Build();

using (var serviceScope = host.Services.CreateScope())
{
    var services = serviceScope.ServiceProvider;
    var syncBlogs = services.GetRequiredService<SyncBlogs>();
    await syncBlogs.Run(); 
}
