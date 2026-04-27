using Microsoft.Extensions.Options;
using Sh.Cache.Redis;
using Sh.Cache.Redis.Controller.Database;
using Sh.Cache.Redis.Controller.Fixture.Live;
using Sh.Cache.Redis.Controller.Fixture.PreMatch;
using Sh.Cache.Redis.Interfaces;
using Sh.LiveWebSocket.MessageHub.Configuration;
using Sh.LiveWebSocket.MessageHub.Hubs;
using Sh.LiveWebSocket.MessageHub.Hubs.Server;
using Sh.LiveWebSocket.MessageHub.Services;
using Sh.LiveWebSocket.MessageHub.Services.Abstractions;
using Sh.Odds.Contracts.Dto.Odds;
using Sh.Odds.Service.Controller;
using Sh.Odds.Service.Controller.WebSocket.Live;
using Sh.Odds.Service.Helpers.Databases;
using Sh.Odds.Service.Interfaces;
using Sh.Odds.Service.Settings;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

//builder.Services.AddHostedService<TestMessageGenerator>();

builder.Services.AddOpenApi();
builder.Services.AddCors();
builder.Services.AddMemoryCache();

builder.Services.AddSignalR();
builder.Services.AddSingleton<MatchMessageBridge>();

builder.Services.AddSingleton<IMatchConnectionStore, MemoryCacheMatchConnectionStore>();

builder.Services.Configure<MongoDbConfiguration>(builder.Configuration.GetSection("MongoDb"));
builder.Services.AddSingleton(sp =>
{
    var options = sp.GetRequiredService<IOptions<MongoDbConfiguration>>();
    return new MongoDbConnectionFactory(options).Database;
});

builder.Services.AddSingleton<ConnectionMultiplexer>(sp =>
{
    var configuration = builder.Configuration.GetConnectionString("Redis") ?? throw new ArgumentNullException("Redis");
    return ConnectionMultiplexer.Connect(configuration);
});

#region NEW

builder.Services.AddSingleton<LiveWebSocketController>();
builder.Services.AddSingleton<PrematchMarketCacheService>();
builder.Services.AddSingleton<LiveMarketCacheService>();
builder.Services.AddSingleton<QuickFixtureController>();
builder.Services.AddSingleton<MongoDbHelper>();
builder.Services.AddSingleton<MongoDatabaseCacheService>();

builder.Services.AddSingleton<IRedisCacheController, RedisCacheController>();

builder.Services.AddSingleton<QuickFixtureController>();
builder.Services.AddSingleton<ILiveProviderPeriodProvider, LiveProviderPeriodProvider>();
builder.Services.AddSingleton<IReadOnlyList<LiveProviderPeriod>>(x => []);
#endregion

var app = builder.Build();

app.UseCors(x => x.WithOrigins("http://localhost:5173").AllowAnyMethod().AllowAnyHeader().AllowCredentials());

app.MapOpenApi();

app.MapHub<ServerMatchHub>("/server/match-hub");

app.MapHub<AllMatchesHub>("/all-matches-hub");
app.MapHub<MatchHub>("/match-hub");

app.Run();