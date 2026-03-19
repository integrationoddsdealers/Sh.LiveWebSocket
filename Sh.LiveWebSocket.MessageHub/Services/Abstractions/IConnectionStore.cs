namespace Sh.LiveWebSocket.MessageHub.Services.Abstractions;

public interface IConnectionStore
{
    ValueTask<string?> GetConnectionGroupAsync(string connectionId);

    ValueTask AddConnectionToGroupAsync(string connectionId, string groupName);

    ValueTask RemoveConnectionAsync(string connectionId);

    ValueTask MoveConnectionToGroupAsync(string connectionId, string newGroupName);
}
