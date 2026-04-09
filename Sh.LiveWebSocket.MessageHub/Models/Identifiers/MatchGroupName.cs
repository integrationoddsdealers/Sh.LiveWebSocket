namespace Sh.LiveWebSocket.MessageHub.Models.Identifiers;

public struct MatchGroupName : IEquatable<MatchGroupName>
{
    public const string Channel = "match";

    public string Name { get; set; }

    public string Language { get; set; }

    public int SiteId { get; set; }

    public MatchGroupName(string language, int siteId)
    {
        Language = language;
        SiteId = siteId;
        Name = $"{Channel}-{Language}-{SiteId}";
    }

    public bool Equals(MatchGroupName other)
    {
        return Name.Equals(other.Name);
    }

    public override string ToString() => Name;
}
