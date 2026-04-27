namespace Sh.LiveWebSocket.MessageHub.Services;

public class TempService
{
    //private readonly ILogger<TempService> _logger;
    //private readonly IMarketRepository _marketRepository;
    //private readonly MatchHubNotification? _matchHubNotification;

    //public async Task SendNotificationAsync(List<FeedMarket> notSettledMarkets, LiveMatch existingMatch, List<LiveOdd> oddList, CancellationToken ct)
    //{
    //    if (_matchHubNotification is null)
    //    {
    //        _logger.LogInformation("MatchHubNotification is not available. Skipping notification.");
    //        return;
    //    }

    //    var marketIds = notSettledMarkets.Select(x => x.GetPandaScoreMarketId()).ToList();

    //    var marketList = new List<Odds.Contracts.MongoDb.Prematch.Odds.Market>(marketIds.Count);
    //    var selectionList = new List<Odds.Contracts.MongoDb.Prematch.Odds.Option>();

    //    foreach (var marketId in marketIds)
    //    {
    //        var marketsResponse = await _marketRepository.GetMarketByIdAsync(marketId, existingMatch.SportId);

    //        if (marketsResponse.IsEmpty)
    //        {
    //            continue;
    //        }

    //        marketList.Add(marketsResponse.Result);

    //        var marketSelections = await _marketRepository.GetSelectionsByMarketIdAsync(marketsResponse.Result.MarketId, marketsResponse.Result.SportId);
    //        selectionList.AddRange(marketSelections.Select(x => x.Result));
    //    }

    //    var participants = new Dictionary<string, string>()
    //    {
    //        { $"sr:competitor:{existingMatch.HomeTeamId}", existingMatch.GetT1Name("en") },
    //        { $"sr:competitor:{existingMatch.AwayTeamId}", existingMatch.GetT2Name("en") }
    //    };

    //    var languages = marketList.SelectMany(x => x.objTrad.Select(t => t.Lang)).Distinct().ToList();

    //    var marketModels = new Dictionary<string, MatchMarketOdds>();

    //    foreach (var language in languages)
    //    {
    //        var result = new MatchMarketOdds();

    //        var marketModelsForLanguage = await TranslationsUtility.CreateMarkets(marketList,
    //            selectionList, oddList.ToList(), language,
    //            participants, existingMatch.Players, IntegrationTypeEnum.Live);

    //        if (!marketModelsForLanguage.Any())
    //        {
    //            continue;
    //        }

    //        var odds = await TranslationsUtility.CreateOdds(marketList, selectionList, oddList.ToList(), language, participants, existingMatch.Players, IntegrationEnum.PandaScore);

    //        result.MatchId = existingMatch.MatchId;
    //        result.Markets = marketModelsForLanguage;
    //        result.Odds = odds;

    //        marketModels.Add(language, result);
    //    }

    //    if (!marketModels.Any())
    //    {
    //        return;
    //    }

    //    await _matchHubNotification.SendMatchesAsync(marketModels, ct);
    //}
}
