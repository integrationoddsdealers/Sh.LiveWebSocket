using Microsoft.AspNetCore.SignalR;
using Sh.LiveWebSocket.MessageHub.Hubs;
using Sh.LiveWebSocket.MessageHub.Messages;
using Sh.LiveWebSocket.MessageHub.Messages.Translated;
using Sh.LiveWebSocket.MessageHub.Services.Abstractions;

namespace Sh.LiveWebSocket.MessageHub.Services;

public sealed class MatchHubBridge
{
    private readonly IHubContext<MatchHub> _hubContext;
    private readonly IMatchConnectionStore _matchConnectionStore;

    public MatchHubBridge(IHubContext<MatchHub> hubContext, IMatchConnectionStore matchConnectionStore)
    {
        _hubContext = hubContext;
        _matchConnectionStore = matchConnectionStore;
    }

    public async Task SendMessageToGroupAsync(string groupName, MatchNotificationMessage message)
    {
        await _hubContext.Clients.Group(groupName).SendAsync(MatchHub.MatchUpdate, message);
    }

    public async Task SendMessagesAsync(Dictionary<string, MatchMarketOdds> message)
    {
        var groups = await _matchConnectionStore.GetAllConnectionGroupsAsync();
        var languages = message.Keys;

        var acceptableGroups = groups.Where(g => languages.Any(lang => g.StartsWith($"match-{lang}-")));

        foreach (var group in acceptableGroups)
        {
            var language = group.Split('-')[1];
            if (message.TryGetValue(language, out var matchMarketOdds))
            {
                await _hubContext.Clients.Group(group).SendAsync(MatchHub.MatchUpdate, matchMarketOdds);
            }
        }
    }
}
