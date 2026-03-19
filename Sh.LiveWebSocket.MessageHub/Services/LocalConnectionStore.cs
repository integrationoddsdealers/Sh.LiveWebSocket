using Sh.LiveWebSocket.MessageHub.Services.Abstractions;
using System.Collections.Concurrent;

namespace Sh.LiveWebSocket.MessageHub.Services;

public sealed class LocalConnectionStore : IConnectionStore
{
    private readonly ConcurrentDictionary<string, string> _connectionGroup = new();

    public ValueTask AddConnectionToGroupAsync(string connectionId, string groupName)
    {
        _connectionGroup.TryAdd(connectionId, groupName);
        return ValueTask.CompletedTask;
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

    public async ValueTask MoveConnectionToGroupAsync(string connectionId, string newGroupName)
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
