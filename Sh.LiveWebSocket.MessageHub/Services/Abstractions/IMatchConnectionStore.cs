using Sh.LiveWebSocket.MessageHub.Models.Identifiers;

namespace Sh.LiveWebSocket.MessageHub.Services.Abstractions;

public interface IMatchConnectionStore
{
    ValueTask<string?> GetConnectionGroupAsync(string connectionId);

    ValueTask<HashSet<string>> GetAllConnectionGroupsAsync();

    ValueTask AddConnectionToGroupAsync(string connectionId, MatchGroupName groupName);

    ValueTask RemoveConnectionAsync(string connectionId);

    ValueTask MoveConnectionToGroupAsync(string connectionId, MatchGroupName newGroupName);
}
