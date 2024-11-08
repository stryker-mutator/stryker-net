namespace Stryker.Core.TestRunners;
public sealed class Identifier : IEquatable<Identifier>
{
    private readonly string _identifier;

    private Identifier(string identifier) => _identifier = identifier;

    public static Identifier Create(string identifier) => new(identifier);

    public static Identifier Create(Guid guid) => new(guid.ToString());

    public Guid ToGuid()
    {
        var isGuid = Guid.TryParse(_identifier, out var guid);
        return isGuid ? guid : Guid.Empty;
    }

    public override int GetHashCode() => _identifier.GetHashCode();

    public override string ToString() => _identifier;

    public override bool Equals(object obj) => obj is Identifier identifier && Equals(identifier);

    public bool Equals(Identifier other) => _identifier == other?._identifier;

    public static bool operator ==(Identifier left, Identifier right) => Equals(left, right);

    public static bool operator !=(Identifier left, Identifier right) => !Equals(left, right);
}
