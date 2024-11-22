using System;
using Stryker.Abstractions.Testing;

namespace Stryker.Core.Mutants;

public sealed class TestDescription : ITestDescription
{
    public TestDescription(Identifier id, string name, string testFilePath)
    {
        Id = id;
        Name = name;
        TestFilePath = testFilePath;
    }

    public Identifier Id { get; }

    public string Name { get; }

    public string TestFilePath { get; }

    private bool Equals(TestDescription other)
    {
        return Id == other.Id;
    }

    public override bool Equals(object obj)
    {
        if (obj is null)
            return false;
        if (ReferenceEquals(this, obj))
            return true;
        return obj.GetType() == GetType() && Equals((TestDescription)obj);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}
