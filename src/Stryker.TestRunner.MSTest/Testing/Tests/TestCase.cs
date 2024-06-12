using Microsoft.Testing.Extensions.TrxReport.Abstractions;
using Microsoft.Testing.Platform.Extensions.Messages;
using Stryker.Shared.Tests;

namespace Stryker.TestRunner.MSTest.Testing.Tests;
internal class TestCase : ITestCase
{
    public TestCase(string source, Uri executor, TestNode testNode)
    {
        var fileLocation = testNode.Properties.Single<FileLocationProperty>();
        var fullyQualifiedName = testNode.Properties.Single<TrxFullyQualifiedTypeNameProperty>();

        Id = Identifier.Create(testNode.Uid);
        Name = testNode.DisplayName;
        Source = source;
        CodeFilePath = fileLocation.FilePath;
        FullyQualifiedName = fullyQualifiedName.FullyQualifiedTypeName;
        Uri = executor;
        LineNumber = fileLocation.LineSpan.Start.Line;
    }

    public Identifier Id { get; }

    public string Name { get; }

    public string Source { get; }

    public string CodeFilePath { get; }

    public string FullyQualifiedName { get; }

    public Uri Uri { get; }

    public int LineNumber { get; }
}
