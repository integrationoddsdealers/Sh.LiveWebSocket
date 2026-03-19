using Microsoft.AspNetCore.SignalR;
using Sh.LiveWebSocket.MessageHub.Hubs;
using Sh.LiveWebSocket.MessageHub.Messages;
using Sh.LiveWebSocket.MessageHub.Messages.Translated;

namespace Sh.LiveWebSocket.MessageHub.Services;

public sealed class MatchHubBridge
{
    private readonly IHubContext<MatchHub> _hubContext;

    public MatchHubBridge(IHubContext<MatchHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task SendMessageToGroupAsync(string groupName, MatchNotificationMessage message)
    {
        await _hubContext.Clients.Group(groupName).SendAsync(MatchHub.MatchUpdate, message);
    }

    public async Task SendMessagesAsync(Dictionary<string, List<MatchMarketModel>> message)
    {
        foreach (var kvp in message)
        {
            var groupName = $"match-{kvp.Key}";
            var matchMarkets = kvp.Value;

            await _hubContext.Clients.Group(groupName).SendAsync(MatchHub.MatchUpdate, matchMarkets);
        }
    }
}
