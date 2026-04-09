namespace Sh.LiveWebSocket.MessageHub.Messages.Translated;

public class MatchOddsModel
{
    public string OddId { get; set; }
    public string OptionId { get; set; }
    public object OddName { get; set; }
    public double OddValue { get; set; }
    public int Mult { get; set; }
    public int Sort { get; set; }
    public int MarketId { get; set; }
    public int ProviderMarketId { get; set; }
    public int OddStatus { get; set; }
    public string Sbv { get; set; }
    public double NSbv { get; set; }
    public string GroupValue1 { get; set; }
    public string GroupValue2 { get; set; }
    public string GroupValue3 { get; set; }
    public long SEId { get; set; }
    public int Sid { get; set; }
}
