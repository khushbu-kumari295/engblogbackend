using EngBlogApi.Manager;
using EngBlogApi.Services;
using EngBlogJob;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var socketsHttpHandler = new SocketsHttpHandler();
socketsHttpHandler.PooledConnectionLifetime = TimeSpan.FromMinutes(5);
builder.Services.AddSingleton(socketsHttpHandler);

builder.Services.AddDbContext<IEngBlogContext, EngBlogContext>(options => options.UseCosmos(
    "https://kksideprojects.documents.azure.com:443/",
    builder.Configuration["CosmosDbKey"],
    "EngBlogDb", (o) =>
    {
        o.HttpClientFactory(() => new HttpClient(socketsHttpHandler, disposeHandler: false));
    }));

builder.Services.AddSingleton<IArticlesManager, ArticlesManager>();

var pollingInterval = Convert.ToInt32(builder.Configuration["PollingInterval"]); 
builder.Services.AddHostedService(provider =>
{
    var articlesManager = provider.GetRequiredService<IArticlesManager>();
    var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
    return new BackgroundPollingService(loggerFactory.CreateLogger<BackgroundPollingService>(), articlesManager, pollingInterval);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
