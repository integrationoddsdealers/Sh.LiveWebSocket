using Microsoft.AspNetCore.SignalR;
using Sh.LiveWebSocket.MessageHub.Models.Identifiers;
using Sh.LiveWebSocket.MessageHub.Services.Abstractions;

namespace Sh.LiveWebSocket.MessageHub.Hubs;

public sealed class MatchHub : Hub
{
    public const string Notifications = "notifications";
    public const string MatchUpdate = "match-update";

    private readonly IMatchConnectionStore _connectionStore;

    public MatchHub(IMatchConnectionStore connectionStore)
    {
        _connectionStore = connectionStore;
    }

    public override async Task OnConnectedAsync()
    {
        var httpContext = Context.GetHttpContext();
        var siteId = httpContext?.Request.Query["siteId"] ?? throw new ArgumentException("siteId");
        var lang = httpContext?.Request.Query["lang"] ?? throw new ArgumentException("lang");

        var groupName = new MatchGroupName(lang.ToString(), int.Parse(siteId.ToString()));

        await Groups.AddToGroupAsync(Context.ConnectionId, groupName.ToString(), Context.ConnectionAborted);
        await base.OnConnectedAsync();

        await Clients.Caller.SendAsync(Notifications, $"You joined to group '{groupName}'.", Context.ConnectionAborted);

        await _connectionStore.AddConnectionToGroupAsync(Context.ConnectionId, groupName);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var groupName = await _connectionStore.GetConnectionGroupAsync(Context.ConnectionId);

        if (!string.IsNullOrEmpty(groupName))
        {
            await _connectionStore.RemoveConnectionAsync(Context.ConnectionId);

            // TODO: Check should I remove from group if connection is aborted by client
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName, Context.ConnectionAborted);
        }

        await base.OnDisconnectedAsync(exception);
    }

    public async Task ChangeLanguage(int siteId, string language)
    {
        var groupName = new MatchGroupName(language, siteId);
        var oldGroup = await _connectionStore.GetConnectionGroupAsync(Context.ConnectionId);

        if (!string.IsNullOrEmpty(oldGroup))
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, oldGroup, Context.ConnectionAborted);
        }

        await Groups.AddToGroupAsync(Context.ConnectionId, groupName.ToString(), Context.ConnectionAborted);

        await _connectionStore.MoveConnectionToGroupAsync(Context.ConnectionId, groupName);
        
        await Clients.Caller.SendAsync(Notifications, $"You joined to group '{groupName}'.", Context.ConnectionAborted);
    }
}
