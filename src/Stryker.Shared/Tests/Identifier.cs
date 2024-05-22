namespace Stryker.Shared.Tests;
public sealed class Identifier : IEquatable<Identifier>
{
    private readonly string _identifier;

    private Identifier(string identifier) => _identifier = identifier;

    public static Identifier Create(string identifier) => new(identifier);

    public static Identifier Create(Guid guid) => new(guid.ToString());


    public static implicit operator Guid(Identifier id)
    {
        var isGuid = Guid.TryParse(id._identifier, out var guid);
        return isGuid ? guid : Guid.Empty;
    }

    public static implicit operator string(Identifier identifier) => identifier._identifier;

    public override int GetHashCode() => _identifier.GetHashCode();

    public override string ToString() => _identifier;

    public override bool Equals(object? obj)
    {
        if (obj is null)
            return false;

        if (ReferenceEquals(this, obj))
            return true;

        return obj.GetType() == GetType() && Equals(obj as Identifier);
    }

    public bool Equals(Identifier? other) => _identifier == other?._identifier;
}
