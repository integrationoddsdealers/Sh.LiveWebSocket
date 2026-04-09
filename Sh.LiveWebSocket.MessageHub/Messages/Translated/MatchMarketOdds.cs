namespace Sh.LiveWebSocket.MessageHub.Messages.Translated;

public class MatchMarketOdds
{
    public long MatchId { get; set; }

    public List<MatchMarketModel> Markets { get; set; } = [];

    public List<MatchOddsModel> Odds { get; set; } = [];
}
