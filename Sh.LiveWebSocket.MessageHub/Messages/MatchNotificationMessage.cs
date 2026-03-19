namespace Sh.LiveWebSocket.MessageHub.Messages;

public class MatchNotificationMessage
{
    public string MessageType { get; set; } = "Match";

    public int MatchId { get; set; }

    public string HomeScore { get; set; } = string.Empty;

    public string AwayScore { get; set; } = string.Empty;

    public string MatchTime { get; set; } = "nd";

    public string? MatchStatus { get; set; }
}
