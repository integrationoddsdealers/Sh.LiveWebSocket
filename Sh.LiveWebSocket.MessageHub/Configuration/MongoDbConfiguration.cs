namespace Sh.LiveWebSocket.MessageHub.Configuration;

public class MongoDbConfiguration
{
    public required string Host { get; set; }

    public int Port { get; set; }

    public required string DatabaseName { get; set; }
}
