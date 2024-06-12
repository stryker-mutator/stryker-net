using Stryker.Shared.Tests;

namespace Stryker.TestRunner.MSTest.Testing.Tests;
internal class TestDescription : ITestDescription
{
    public TestDescription(string id, string name, string testFilePath)
    {
        Id = Identifier.Create(id);
        Name = name;
        TestFilePath = testFilePath;
    }

    public Identifier Id { get; }

    public string Name { get; }

    public string TestFilePath { get; }
}
