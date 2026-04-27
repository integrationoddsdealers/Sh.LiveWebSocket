using Microsoft.AspNetCore.SignalR;
using Sh.LiveWebSocket.MessageHub.Services;
using Sh.Odds.Contracts.WebSocket.Live.TransferMessages;

namespace Sh.LiveWebSocket.MessageHub.Hubs.Server;

public sealed class ServerMatchHub : Hub
{
    private readonly MatchMessageBridge _matchMessageBridge;

    public ServerMatchHub(MatchMessageBridge matchMessageBridge)
    {
        _matchMessageBridge = matchMessageBridge;
    }

    public async Task MatchUpdates(LiveOddsTransferMessage message)
    {
       await _matchMessageBridge.SendOddsMessagesAsync(message);
    }

    public async Task DisableOdds(DisableOddsTransferMessage message)
    {
        await _matchMessageBridge.SendDisableOddsMessagesAsync(message);
    }
}
