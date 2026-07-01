using System;
using System.Diagnostics.CodeAnalysis;
using Stryker.Abstractions.Testing;
namespace Stryker.TestRunner.MicrosoftTestPlatform.Models;

[ExcludeFromCodeCoverage]
public sealed class MtpTestCase : ITestCase
{
    private readonly TestNode _testNode;
    public MtpTestCase(TestNode testNode)
    {
        _testNode = testNode;
        CodeFilePath = testNode.LocationFile ?? string.Empty;
        // If the test framework doesn't provide location info, we will use -1 to indicate that the line number is unknown.
        // ProjectMutator will try to find the test method by name in this case.
        LineNumber = testNode.LocationLineStart ?? -1;
        FullyQualifiedName = BuildFullyQualifiedName(testNode);
    }

    public string FullyQualifiedName { get; }
    public Uri Uri => new("executor://MicrosoftTestPlatform");
    public int LineNumber { get; }

    public string Source { get; init; } = string.Empty;
    public string CodeFilePath { get; }

    public string AssemblyPath { get; init; } = string.Empty;

    public Guid Guid { get; }
    public string Name => _testNode.DisplayName;

    public string Id => _testNode.Uid;

    private static string BuildFullyQualifiedName(TestNode testNode)
    {
        if (testNode.LocationType is not null && testNode.LocationMethod is not null)
        {
            return $"{testNode.LocationType}.{testNode.LocationMethod}";
        }

        return testNode.Uid;
    }
}
