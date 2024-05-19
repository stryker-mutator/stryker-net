namespace Stryker.Shared.Tests;
public sealed class Identifier
{
    private readonly string _identifier;

    private Identifier(string identifier) => _identifier = identifier;

    public static Identifier Create(string identifier) => new(identifier);

    public static Identifier Create(Guid guid) => new(guid.ToString());


    public static explicit operator Guid(Identifier id)
    {
        var isGuid = Guid.TryParse(id._identifier, out var guid);
        return isGuid ? guid : Guid.Empty;
    }

    public static explicit operator string(Identifier identifier) => identifier._identifier;

    public override int GetHashCode() => _identifier.GetHashCode();

    public override string ToString() => _identifier;
}
