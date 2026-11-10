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
        // If the test framework doesn't provide location info, we will use LineNumberNotFound to indicate that the line number is unknown.
        // ProjectMutator will try to find the test method by name in this case.
        LineNumber = testNode.LocationLineStart ?? ITestCase.LineNumberNotFound;
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
            // MTP reports the method with its parameter types, e.g. "TestMethod(System.Int32)" for a theory.
            // Drop them so the name matches the fully qualified name VsTest reports for the same test.
            var parameterListStart = testNode.LocationMethod.IndexOf('(');
            var methodName = parameterListStart >= 0
                ? testNode.LocationMethod[..parameterListStart]
                : testNode.LocationMethod;
            return $"{testNode.LocationType}.{methodName}";
        }

        return testNode.Uid;
    }
}
