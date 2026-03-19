using Sh.LiveWebSocket.MessageHub.Hubs;
using Sh.LiveWebSocket.MessageHub.Hubs.Server;
using Sh.LiveWebSocket.MessageHub.Services;
using Sh.LiveWebSocket.MessageHub.Services.Abstractions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddSignalR();
builder.Services.AddSingleton<MatchHubBridge>();

builder.Services.AddSingleton<IConnectionStore, LocalConnectionStore>();

var app = builder.Build();

app.MapOpenApi();

app.MapHub<ServerMatchHub>("/server/match-hub");

app.MapHub<MatchHub>("/match-hub");

app.Run();
