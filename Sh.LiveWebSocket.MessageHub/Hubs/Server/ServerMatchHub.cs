using Microsoft.AspNetCore.SignalR;
using Sh.LiveWebSocket.MessageHub.Messages.Translated;
using Sh.LiveWebSocket.MessageHub.Services;

namespace Sh.LiveWebSocket.MessageHub.Hubs.Server;

public sealed class ServerMatchHub : Hub
{
    private readonly MatchHubBridge _matchHubNotificationService;

    public ServerMatchHub(MatchHubBridge matchHubNotificationService)
    {
        _matchHubNotificationService = matchHubNotificationService;
    }

    public async Task MatchUpdates(Dictionary<string, List<MatchMarketModel>> matches)
    {
       await _matchHubNotificationService.SendMessagesAsync(matches);
    }
}
