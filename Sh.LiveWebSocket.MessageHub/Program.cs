using Sh.LiveWebSocket.MessageHub;
using Sh.LiveWebSocket.MessageHub.Hubs;
using Sh.LiveWebSocket.MessageHub.Hubs.Server;
using Sh.LiveWebSocket.MessageHub.Services;
using Sh.LiveWebSocket.MessageHub.Services.Abstractions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHostedService<TestMessageGenerator>();

builder.Services.AddOpenApi();
builder.Services.AddCors();
builder.Services.AddMemoryCache();

builder.Services.AddSignalR();
builder.Services.AddSingleton<MatchHubBridge>();

builder.Services.AddSingleton<IMatchConnectionStore, MemoryCacheMatchConnectionStore>();

var app = builder.Build();


app.UseCors(x => x.WithOrigins("http://localhost:5173").AllowAnyMethod().AllowAnyHeader().AllowCredentials());

app.MapOpenApi();

app.MapHub<ServerMatchHub>("/server/match-hub");

app.MapHub<MatchHub>("/match-hub");

app.Run();
