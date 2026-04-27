using Microsoft.AspNetCore.SignalR;
using Sh.Cache.Redis.Controller.Database;
using Sh.LiveWebSocket.MessageHub.Hubs;
using Sh.LiveWebSocket.MessageHub.Services.Abstractions;
using Sh.Odds.Contracts.WebSocket.Live;
using Sh.Odds.Contracts.WebSocket.Live.TransferMessages;
using Sh.Odds.Service.Controller.WebSocket.Live;
using Sh.Odds.Service.Helpers.Databases;

namespace Sh.LiveWebSocket.MessageHub.Services;

public sealed class MatchMessageBridge
{
    private List<string>? _languages;

    private readonly IHubContext<AllMatchesHub> _allMatchesHubContext;
    private readonly IHubContext<MatchHub> _matchHubContext;
    private readonly IMatchConnectionStore _matchConnectionStore;
    private readonly LiveWebSocketController _liveWebSocketController;
    private readonly MongoDatabaseCacheService _mongoDatabaseCacheService;
    private readonly MongoDbHelper _mongoDbHelper;
    private readonly IMatchConnectionStore _connectionStore;

    public MatchMessageBridge(
        IHubContext<AllMatchesHub> allMatchesHubContext,
        IHubContext<MatchHub> matchHubContext,
        IMatchConnectionStore matchConnectionStore,
        LiveWebSocketController liveWebSocketController,
        MongoDatabaseCacheService mongoDatabaseCacheService,
        MongoDbHelper mongoDbHelper,
        IMatchConnectionStore connectionStore)
    {
        _allMatchesHubContext = allMatchesHubContext;
        _matchHubContext = matchHubContext;
        _matchConnectionStore = matchConnectionStore;
        _liveWebSocketController = liveWebSocketController;
        _mongoDatabaseCacheService = mongoDatabaseCacheService;
        _mongoDbHelper = mongoDbHelper;
        _connectionStore = connectionStore;
    }

    public async Task SendOddsMessagesAsync(LiveOddsTransferMessage message)
    {
        await GetLanguagesIfNotExistAsync();

        foreach (var language in _languages ?? [])
        {
            if (string.IsNullOrEmpty(language))
            {
                continue;
            }

            var liveOddMessage = await _liveWebSocketController.GetWsOddsMessage(message.Markets, message.NewOdds, message.UpdatedOdds, message.Match, Odds.Contracts.Enum.IntegrationEnum.PandaScore, "en");

            await _allMatchesHubContext.Clients.Group($"match-*-{language}-1").SendAsync(AllMatchesHub.AllMatchesUpdate, liveOddMessage);
            await _matchHubContext.Clients.Group($"match-{message.Match.MatchId}-{language}-1").SendAsync(MatchHub.MatchUpdate, liveOddMessage);
        }
    }

    public async Task SendDisableOddsMessagesAsync(DisableOddsTransferMessage message)
    {
        await GetLanguagesIfNotExistAsync();

        foreach (var language in _languages ?? [])
        {
            if (message.MatchId is null)
            {
                await _allMatchesHubContext.Clients.Group($"match-*-{language}-1").SendAsync(AllMatchesHub.DisableOdds, message.MatchId);
            }
            else
            {
                await _matchHubContext.Clients.Group($"match-{message.MatchId}-{language}-1").SendAsync(MatchHub.DisableOdds, true);
            }
        }

        if (message.MatchId is null)
        {
            var groups = await _connectionStore.GetAllConnectionGroupsAsync();
            foreach (var group in groups)
            {
                await _matchHubContext.Clients.Group(group).SendAsync(MatchHub.DisableOdds, true);
            }
        }
    }

    private async Task GetLanguagesIfNotExistAsync()
    {
        if (_languages is null)
        {
            var integrationContext = await _mongoDbHelper.ResolveIntegrationContext(Odds.Contracts.Enum.IntegrationEnum.PandaScore, Odds.Contracts.Enum.IntegrationTypeEnum.Live);
            var oddsCollection = await _mongoDatabaseCacheService.GetArchiveCacheAsync(integrationContext.ArchiveId);
            _languages = oddsCollection.Languages;
        }
    }
}
