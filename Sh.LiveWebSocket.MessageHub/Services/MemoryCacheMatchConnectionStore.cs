using Microsoft.Extensions.Caching.Memory;
using Sh.LiveWebSocket.MessageHub.Models.Identifiers;
using Sh.LiveWebSocket.MessageHub.Services.Abstractions;

namespace Sh.LiveWebSocket.MessageHub.Services;

public class MemoryCacheMatchConnectionStore : IMatchConnectionStore
{
    private static TimeSpan DefaultExpirationTime => TimeSpan.FromHours(1);

    private readonly MemoryCache _memoryCache;

    public MemoryCacheMatchConnectionStore(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache as MemoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
    }

    public ValueTask AddConnectionToGroupAsync(string connectionId, MatchGroupName groupName)
    {
        _memoryCache.Set(connectionId, groupName.ToString(), DefaultExpirationTime);
        return ValueTask.CompletedTask;
    }

    public ValueTask<string?> GetConnectionGroupAsync(string connectionId)
    {
        var item = _memoryCache.Get<string?>(connectionId);

        if (item is null)
        {
            return ValueTask.FromResult<string?>(null);
        }

        return ValueTask.FromResult<string?>(item);
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
        _memoryCache.Remove(connectionId);
        return ValueTask.CompletedTask;
    }

    public async ValueTask<HashSet<string>> GetAllConnectionGroupsAsync()
    {
        var groups = new HashSet<string>();

        var keys = _memoryCache.Keys.OfType<string>();

        if (!keys.Any())
        {
            return [];
        }

        foreach (var connectionId in keys)
        {
            var group = await GetConnectionGroupAsync(connectionId);

            if (!string.IsNullOrEmpty(group))
            {
                groups.Add(group);
            }
        }

        return groups;
    }
}
