using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Sh.LiveWebSocket.MessageHub.Configuration;

namespace Sh.LiveWebSocket.MessageHub.Services;

public class MongoDbConnectionFactory
{
    private readonly MongoClient _client;
    private readonly MongoDbConfiguration _mongoDbConfiguration;

    public MongoDbConnectionFactory(IOptions<MongoDbConfiguration> mongoDbConfiguration)
    {
        _mongoDbConfiguration = mongoDbConfiguration.Value;
        var connectionString = $"mongodb://{_mongoDbConfiguration.Host}:{_mongoDbConfiguration.Port}/${_mongoDbConfiguration.DatabaseName}";

        _client = new MongoClient(connectionString);
    }

    public IMongoDatabase Database => _client.GetDatabase(_mongoDbConfiguration.DatabaseName);
}
