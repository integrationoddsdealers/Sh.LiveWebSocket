using Sh.PandaScore.WebSockets.Messages.Translated;

namespace Sh.LiveWebSocket.MessageHub.Messages.Translated;

public class MatchMarketModel
{
    public long MarketId { get; set; }
    public string Name { get; set; }
    public bool HasSpread { get; set; }
    public int Sort { get; set; }
    public int SelectionType { get; set; }
    public List<MatchOptionModel> QOptions { get; set; }
    public string Sbv { get; set; }
    public int MarketType { get; set; }
    public string GroupValue1 { get; set; }
    public string GroupValue2 { get; set; }
    public string GroupValue3 { get; set; }
    public int OutcomeCount { get; set; }
    public int SportId { get; set; }
    public List<OfferTypesModel> OfferTypes { get; set; }
    public long OverrideMarket { get; set; }
    public long EventId { get; set; }
}
