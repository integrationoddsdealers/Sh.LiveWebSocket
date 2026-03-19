namespace Sh.LiveWebSocket.MessageHub.Messages;

public sealed class SportResultMessage
{
    public required string Videogame { get; set; }

    public required string Score { get; set; }

    public int CurrentPeriod { get; set; }

    public List<Player> Players { get; set; } = [];

    public class Player
    {
        public int Id { get; set; }

        public int Points { get; set; }
    }
}
