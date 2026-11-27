namespace Stryker.Abstractions.Testing;

public sealed class TestDescription(string id, string name, string testFilePath) : ITestDescription
{
    public string Id { get; } = id;

    public string Name { get; } = name;

    public string TestFilePath { get; } = testFilePath;

    private bool Equals(TestDescription other) => Id == other.Id;

    public override bool Equals(object obj) =>
        obj is not null && (ReferenceEquals(this, obj)
                            || (obj.GetType() == GetType() && Equals((TestDescription)obj)));

    public override int GetHashCode() => Id.GetHashCode();
}
