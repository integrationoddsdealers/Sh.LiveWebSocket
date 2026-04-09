using Sh.LiveWebSocket.MessageHub.Models.Identifiers;
using Sh.LiveWebSocket.MessageHub.Services.Abstractions;
using System.Collections.Concurrent;

namespace Sh.LiveWebSocket.MessageHub.Services;

public sealed class LocalMatchConnectionStore : IMatchConnectionStore
{
    private readonly ConcurrentDictionary<string, string> _connectionGroup = new();

    public ValueTask AddConnectionToGroupAsync(string connectionId, MatchGroupName groupName)
    {
        _connectionGroup.TryAdd(connectionId, groupName.ToString());
        return ValueTask.CompletedTask;
    }

    public ValueTask<HashSet<string>> GetAllConnectionGroupsAsync()
    {
        return ValueTask.FromResult(new HashSet<string>(_connectionGroup.Values));
    }

    public ValueTask<string?> GetConnectionGroupAsync(string connectionId)
    {
        var exists = _connectionGroup.TryGetValue(connectionId, out var groupName);

        if (!exists)
        {
            return ValueTask.FromResult<string?>(null);
        }

        return ValueTask.FromResult(groupName);
    }

    public async ValueTask MoveConnectionToGroupAsync(string connectionId, MatchGroupName newGroupName)
    {
        var currentGroup = await GetConnectionGroupAsync(connectionId);

        if (!string.IsNullOrEmpty(currentGroup))
        {
            await RemoveConnectionAsync(connectionId);
        }

        await AddConnectionToGroupAsync(connectionId, newGroupName);
    }

    public ValueTask RemoveConnectionAsync(string connectionId)
    {
        _connectionGroup.TryRemove(connectionId, out var _);
        return ValueTask.CompletedTask;
    }
}
