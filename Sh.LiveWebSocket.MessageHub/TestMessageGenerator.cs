using Microsoft.AspNetCore.SignalR;
using Sh.LiveWebSocket.MessageHub.Hubs;
using Sh.LiveWebSocket.MessageHub.Messages.Translated;
using Sh.LiveWebSocket.MessageHub.Models.Identifiers;
using Sh.PandaScore.WebSockets.Messages.Translated;

namespace Sh.LiveWebSocket.MessageHub;

public class TestMessageGenerator : BackgroundService
{
    public static string[] AvailableLanguages = ["en", "it", "es", "fr", "de"];

    private readonly IHubContext<MatchHub> _hubContext;

    public TestMessageGenerator(IHubContext<MatchHub> hubContext)
    {
        _hubContext = hubContext;
    }


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(5000, stoppingToken);
        var random = new Random();

        while (!stoppingToken.IsCancellationRequested)
        {
            var message = new Dictionary<string, MatchMarketOdds>();

            foreach (var language in AvailableLanguages)
            {
                var matchMarketOdds = new MatchMarketOdds
                {
                    MatchId = random.Next(1, 10)
                };

                var matchMarkets = new List<MatchMarketModel>();

                for (int i = 0; i < random.Next(1, 5); i++)
                {
                    matchMarkets.Add(new MatchMarketModel
                    {
                        MarketId = random.NextInt64(1, 30),
                        Name = $"Market {language} {random.Next(1, 50)}",
                        HasSpread = random.Next(0, 2) == 0,
                        Sort = random.Next(1, 10),
                        SelectionType = random.Next(1, 5),
                        QOptions = [],
                        Sbv = $"SBV {language}",
                        MarketType = random.Next(1, 3),
                        GroupValue1 = $"GroupValue1 {language}",
                        GroupValue2 = $"GroupValue2 {language}",
                        GroupValue3 = $"GroupValue3 {language}",
                        OutcomeCount = random.Next(1, 5),
                        SportId = random.Next(1, 10),
                        OfferTypes = [],
                        OverrideMarket = random.NextInt64(1, 1000),
                        EventId = random.NextInt64(1, 1000)
                    });
                }

                matchMarketOdds.Markets.AddRange(matchMarkets);

                var matchOdds = new List<MatchOddsModel>();

                matchMarkets.ForEach(m =>
                {
                    for (int i = 0; i < random.Next(1, 10); i++)
                    {
                        matchOdds.Add(new MatchOddsModel
                        {
                            OddId = $"OddId {language} {random.Next(1, 1000)}",
                            OptionId = $"OptionId {language} {random.Next(1, 1000)}",
                            OddName = $"OddName {language} {random.Next(1, 1000)}",
                            OddValue = random.NextDouble() * 10,
                            Mult = random.Next(1, 5),
                            Sort = random.Next(1, 10),
                            MarketId = (int)m.MarketId,
                            ProviderMarketId = random.Next(1, 30),
                            OddStatus = random.Next(0, 3),
                            Sbv = $"SBV {language}",
                            NSbv = random.NextDouble() * 10,
                            GroupValue1 = $"GroupValue1 {language}",
                            GroupValue2 = $"GroupValue2 {language}",
                            GroupValue3 = $"GroupValue3 {language}",
                            SEId = random.NextInt64(1, 1000),
                            Sid = random.Next(1, 10)
                        });
                    }
                });

                matchMarketOdds.Odds.AddRange(matchOdds);

                message[language] = matchMarketOdds;
            }

            foreach (var kvp in message)
            {
                var groupName = new MatchGroupName(kvp.Key, 1);
                var matchMarkets = kvp.Value;

                await _hubContext.Clients.Group(groupName.ToString()).SendAsync(MatchHub.MatchUpdate, matchMarkets);
            }

            await Task.Delay(random.Next(3_000, 10_000), stoppingToken);
        }
    }
}
